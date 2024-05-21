using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace MBTExample
{
    [MBTNode("Example/BossAttack")]
    [AddComponentMenu("")]
    public class BossAttackNode : Leaf
    {
        
        
        public GameObjectReference boss;

        public bool attack;
        public override NodeResult Execute()
        {
            if (boss.Value != null)
            {
                boss.Value.GetComponent<Animator>().SetBool("Attack", attack);
                return NodeResult.success;
            }
            else
            {
                return NodeResult.failure;
            }
        }

    }
}