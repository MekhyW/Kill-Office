﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "Decorators/Cooldown")]
    public class Cooldown : Decorator, IMonoBehaviourTreeTickListener
    {
        public AbortTypes abort = AbortTypes.None;
        [Space]
        public FloatReference time = new FloatReference(1f);
        [Tooltip("When set to true, there will be no cooldown when child node returns failure")]
        public bool resetOnChildFailure = false;
        private float cooldownTime = 0f;
        private bool entered = false;
        private bool childFailed = false;
        
        public enum AbortTypes
        {
            None, LowerPriority
        }

        public override void OnAllowInterrupt()
        {
            if (abort == AbortTypes.LowerPriority)
            {
                ObtainTreeSnapshot();
            }
        }

        public override NodeResult Execute()
        {
            if (!TryGetChild(out Node node))
            {
                return NodeResult.failure;
            }
            if (node.status == Status.Success) {
                return NodeResult.success;
            }
            if (node.status == Status.Failure) {
                // If reset option is enabled flag will be raised and set true
                childFailed = resetOnChildFailure;
                return NodeResult.failure;
            }
            if (cooldownTime <= Time.time) {
                entered = true;
                return node.runningNodeResult;
            } else {
                return NodeResult.failure;
            }
        }

        public override void OnExit()
        {
            // Setup cooldown and event when child was entered
            // Check reset option too
            if (entered && !childFailed)
            {
                cooldownTime = Time.time + time.Value;
                // For LowerPriority try to abort after given time
                if (abort == AbortTypes.LowerPriority)
                {
                    behaviourTree.AddTickListener(this);
                }
            }
            // Reset flags
            entered = false;
            childFailed = false;
        }

        public override void OnDisallowInterrupt()
        {
            behaviourTree.RemoveTickListener(this);
        }

        void IMonoBehaviourTreeTickListener.OnBehaviourTreeTick()
        {
            if (cooldownTime <= Time.time)
            {
                // Task should be aborted, so there is no need to listen anymore
                behaviourTree.RemoveTickListener(this);
                TryAbort(Abort.LowerPriority);
            }
        }
    }
}
