using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace MBTExample
{
    [MBTNode("Example/BossFl")]
    [AddComponentMenu("")]
    public class BossFlip : Leaf
    {
        public GameObjectReference boss;
        public GameObjectReference player;

        void Update()
    {
        // Verifica se os Transforms do jogador e do boss foram atribuídos
        if (player != null && boss != null)
        {
            // Obtém a posição do jogador e do boss
            Vector3 playerPosition = player.Value.transform.position;
            Vector3 bossPosition = boss.Value.transform.position;

            // Calcula a direção na qual o boss deve olhar
            Vector3 direction = playerPosition - bossPosition;

            // Verifica se a direção é para a esquerda ou para a direita
            if (direction.x < 0)
            {
                // Olhando para a esquerda
                boss.Value.transform.localScale = new Vector3(-1, 1, 1); // Inverte a escala no eixo X
            }
            else
            {
                // Olhando para a direita
                boss.Value.transform.localScale= new Vector3(1, 1, 1); // Mantém a escala padrão
            }
        }
    }

        public override NodeResult Execute()
        {
            if (boss.Value != null && player.Value != null)
            {
                return NodeResult.success;
            }
            else
            {
                return NodeResult.failure;
            }
        }
    }
}