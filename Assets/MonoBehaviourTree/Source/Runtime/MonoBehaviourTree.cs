﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Profiling;

namespace MBT
{
    [DisallowMultipleComponent]
    // [RequireComponent(typeof(Blackboard))]
    public class MonoBehaviourTree : MonoBehaviour
    {
        private static readonly ProfilerMarker _TickMarker = new ProfilerMarker("MonoBehaviourTree.Tick");

        [HideInInspector]
        public Node selectedEditorNode;
        public bool repeatOnFinish = false;
        public int maxExecutionsPerTick = 1000;
        public MonoBehaviourTree parent;
        
        /// <summary>
        /// Event triggered when tree is about to be updated
        /// </summary>
        public event UnityAction onTick;
        private List<IMonoBehaviourTreeTickListener> tickListeners = new List<IMonoBehaviourTreeTickListener>();
        private Root rootNode;
        private List<Node> executionStack;
        private List<Node> executionLog;
        private List<Decorator> interruptingNodes = new List<Decorator>();
        public float LastTick { get; internal set; }

        void Awake()
        {
            rootNode = GetComponent<Root>();
            if (rootNode == null) {
                Debug.LogWarning("Missing Root node in behaviour tree.", this);
            }
            
            // Find master parent tree and all nodes
            MonoBehaviourTree masterTree = this.GetMasterTree();
            Node[] nodes = GetComponents<Node>();
            if(masterTree == this)
            {
                // Create lists with capicity
                executionStack = new List<Node>(8);
                executionLog = new List<Node>(nodes.Length);
                // Set start node when tree is created first time
                executionStack.Add(rootNode);
                executionLog.Add(rootNode);
            }
            // Initialize nodes of tree/subtree
            for (int i = 0; i < nodes.Length; i++)
            {
                Node n = nodes[i];
                n.behaviourTree = masterTree;
                n.runningNodeResult = new NodeResult(Status.Running, n);
            }
        }

        private void EvaluateInterruptions()
        {
            if (interruptingNodes.Count == 0) {
                return;
            }

            // Find node with highest priority - closest to the root (the smallest number)
            Decorator abortingNode = interruptingNodes[0];
            for (int i = 1; i < interruptingNodes.Count; i++)
            {
                Decorator d = interruptingNodes[i];
                if (d.runtimePriority < abortingNode.runtimePriority) {
                    abortingNode = d;
                }
            }

            // Revert stack
            executionStack.Clear();
            executionStack.AddRange(abortingNode.GetStoredTreeSnapshot());
            // Restore flow of events in nodes after abort
            for (int i = 0; i < executionStack.Count; i++)
            {
                Node node = executionStack[i];
                if (node.status == Status.Running)
                {
                    // This node is still running and might need to restore the state
                    node.OnBehaviourTreeAbort();
                }
                else if (node.status == Status.Success || node.status == Status.Failure)
                {
                    // This node returned failure or success, so reenter it and call OnEnter
                    node.OnEnter();
                }
                // All nodes in execution stack should be in running state
                node.status = Status.Running;
            }
            
            int nodeIndex = abortingNode.runtimePriority - 1;
            // Sanity check
            if (abortingNode != executionLog[nodeIndex]) {
                Debug.LogWarning("Priority of node does not match with exectuion log");
            }
            // Abort nodes in log
            ResetNodesTo(abortingNode, true);
            // Reset aborting node
            abortingNode.status = Status.Ready;
            // Reset list and wait for new interruptions
            interruptingNodes.Clear();
        }

        /// <summary>
        /// Update tree state.
        /// </summary>
        public void Tick()
        {
            _TickMarker.Begin();
            // Fire Tick event and notify listeners
            onTick?.Invoke();
            for (int i = 0; i < tickListeners.Count; i++)
            {
                tickListeners[i].OnBehaviourTreeTick();
            }
            
            // Check if there are any interrupting nodes
            EvaluateInterruptions();

            // Max number of traversed nodes
            int executionLimit = maxExecutionsPerTick;
            // Traverse tree
            while (executionStack.Count > 0)
            {
                if (executionLimit == 0) {
                    LastTick = Time.time;
                    _TickMarker.End();
                    return;
                }
                executionLimit -= 1;

                // Execute last element in stack
                Node currentNode = executionStack[executionStack.Count - 1];
                NodeResult nodeResult = currentNode.Execute();
                // Set new status
                currentNode.status = nodeResult.status;
                if (nodeResult.status == Status.Running) {
                    // If node is running, then stop execution or continue children
                    Node child = nodeResult.child;
                    if (child == null) {
                        // Stop execution and continue next tick
                        LastTick = Time.time;
                        _TickMarker.End();
                        return;
                    } else {
                        // Add child to execution stack and execute it in next loop
                        executionStack.Add(child);
                        executionLog.Add(child);
                        // IMPORTANT: Priority must be > 0 and assigned in this order
                        child.runtimePriority = executionLog.Count;
                        child.OnAllowInterrupt();
                        child.OnEnter();
                        #if UNITY_EDITOR
                        // Stop execution if breakpoint is set on this node
                        if (child.breakpoint)
                        {
                            Debug.Break();
                            UnityEditor.Selection.activeGameObject = this.gameObject;
                            Debug.Log("MBT Breakpoint: " + child.title, this);
                            LastTick = Time.time;
                            _TickMarker.End();
                            return;
                        }
                        #endif
                        continue;
                    }
                } else {
                    // Remove last node from stack and move up (closer to root)
                    currentNode.OnExit();
                    executionStack.RemoveAt(executionStack.Count - 1);
                }
            }
            
            // Run this when execution stack is empty and BT should repeat
            if (repeatOnFinish) {
                Restart();
            }

            LastTick = Time.time;
            _TickMarker.End();
        }

        public void AddTickListener(IMonoBehaviourTreeTickListener listener)
        {
#if UNITY_EDITOR
            if (tickListeners.Contains(listener))
            {
                Debug.LogErrorFormat(this, "Tick listener {0} has been already added.", listener);
            }
#endif
            tickListeners.Add(listener);
        }

        public void RemoveTickListener(IMonoBehaviourTreeTickListener listener)
        {
            tickListeners.Remove(listener);
        }

        /// <summary>
        /// This method should be called to abort tree to given node
        /// </summary>
        /// <param name="node">Abort and revert tree to this node</param>
        internal void Interrupt(Decorator node)
        {
            if (!interruptingNodes.Contains(node)) {
                interruptingNodes.Add(node);
            }
        }

        internal void ResetNodesTo(Node node, bool aborted = false)
        {
            int i = executionLog.Count - 1;
            // Reset status and get index of node
            while (i >= 0)
            {
                Node n = executionLog[i];
                if (n == node) {
                    break;
                }
                // If node is running (on exec stack) then call exit
                if (n.status == Status.Running) {
                    n.OnExit();
                    // IMPROVEMENT: Abort event can be added or abort param onExit
                }
                n.status = Status.Ready;
                n.OnDisallowInterrupt();
                i -= 1;
            }
            // Reset log
            i += 1;
            if (i >= executionLog.Count) {
                return;
            }
            executionLog.RemoveRange(i, executionLog.Count - i);
        }

        private void ResetNodes()
        {
            for (int i = 0; i < executionLog.Count; i++)
            {
                Node node = executionLog[i];
                if (node.status == Status.Running)
                {
                    node.OnExit();
                }
                node.OnDisallowInterrupt();
                node.status = Status.Ready;
            }
            executionLog.Clear();
            executionStack.Clear();
        }

        /// <summary>
        /// Resets state to root node
        /// </summary>
        public void Restart()
        {
            ResetNodes();
            executionStack.Add(rootNode);
            executionLog.Add(rootNode);
        }

        internal void GetStack(ref Node[] stack)
        {
            // Resize array when size is too small
            if (executionStack.Count > stack.Length)
            {
                // Node should not change priority and position during runtime
                // It means the array will be resized once during first call of this method
                Array.Resize<Node>(ref stack, executionStack.Count);
            }
#if UNITY_EDITOR
            // Additional sanity check in case nodes are reordered or changed in editor
            if (stack.Length > executionStack.Count)
            {
                Debug.LogError("Changing order of MBT nodes during runtime might cause errors or unpredictable results.");
            }
#endif
            // Copy elements to provided array
            executionStack.CopyTo(stack);
        }

        public Node GetRoot()
        {
            return rootNode;
        }

        public MonoBehaviourTree GetMasterTree()
        {
            if (parent == null)
            {
                return this;
            }
            return parent.GetMasterTree();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (maxExecutionsPerTick <= 0)
            {
                maxExecutionsPerTick = 1;
            }
            
            if (parent != null)
            {
                if (parent == this)
                {
                    parent = null;
                    Debug.LogWarning("This tree cannot be its own parent.");
                    return;
                }
                if (transform.parent == null || parent.gameObject != transform.parent.gameObject)
                {
                    // parent = null;
                    Debug.LogWarning("Parent tree should be also parent of this gameobject.", this.gameObject);
                }
            }
        }
#endif
    }
}
