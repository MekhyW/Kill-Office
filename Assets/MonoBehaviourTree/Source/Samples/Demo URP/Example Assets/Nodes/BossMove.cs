  using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace MBTExample
{
    [MBTNode("Example/BossMove")]
    [AddComponentMenu("")]
    public class BossMove : Leaf
    {
        public GameObjectReference boss;
        public GameObjectReference player;
        public bool attack;

        public override NodeResult Execute()
        {
            if (boss.Value != null && player.Value != null)
            {
                Transform bossTransform = boss.Value.transform;
                // colocar trigger no animator do boss na variavel move
                Transform playerTransform = player.Value.transform;
                bossTransform.position = playerTransform.position; // Fix: Assign the position property instead of the entire transform
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