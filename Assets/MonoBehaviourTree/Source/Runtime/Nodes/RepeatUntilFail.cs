﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode("Decorators/Repeat Until Fail")]
    public class RepeatUntilFail : Decorator
    {
        public override NodeResult Execute()
        {
            if (!TryGetChild(out Node node))
            {
                return NodeResult.failure;
            }
            if (node.status == Status.Failure) {
                return NodeResult.success;
            }
            // Repeat children
            behaviourTree.ResetNodesTo(this);
            return node.runningNodeResult; 
        }
    }
}
