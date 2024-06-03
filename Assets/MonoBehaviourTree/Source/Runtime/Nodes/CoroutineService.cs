﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    public abstract class CoroutineService : Decorator
    {
        public float interval = 1f;
        public bool callOnEnter = true;
        protected Coroutine coroutine;
        private WaitForSeconds waitForSeconds;

        public override void OnEnter()
        {
            // IMPROVEMENT: WaitForSeconds could be initialized in some special node init callback
            if (waitForSeconds == null)
            {
                // Create new WaitForSeconds
                OnValidate();
            }
            coroutine = StartCoroutine(ScheduleTask());
            if (callOnEnter)
            {
                Task();
            }
        }

        public override NodeResult Execute()
        {
            Node node = GetChild();
            if (node == null) {
                return NodeResult.failure;
            }
            if (node.status == Status.Success || node.status == Status.Failure) {
                return NodeResult.From(node.status);
            }
            return node.runningNodeResult;
        }

        public abstract void Task();

        public override void OnExit()
        {
            if (coroutine == null)
            {
                return;
            }
            StopCoroutine(coroutine);
            coroutine = null;
        }

        private IEnumerator ScheduleTask()
        {
            while(true)
            {
                yield return waitForSeconds;
                Task();
            }
        }

        protected virtual void OnValidate()
        {
            interval = Mathf.Max(0f, interval);
            waitForSeconds = new WaitForSeconds(interval);
        }
    }
}
