using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactiveLaser : MonoBehaviour
{
    // Start is called before the first frame update
    public float yLimit = -10.0f;

    void Update()
    {
        // Verifica se a posição Y do inimigo é menor que o limite
        if (transform.position.y < yLimit)
        {
            // Desativa o GameObject
            Destroy(gameObject);
        }
    }
    
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
