using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    // Start is called before the first frame update

        private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o objeto que entrou no trigger é o jogador
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            // Chama o método killPlayer no jogador
            player.killPlayer();
        }

    }
}
