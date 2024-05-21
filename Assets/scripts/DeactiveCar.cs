using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactiveCar : MonoBehaviour
{
    // Start is called before the first frame update
    public float xLimit = 35f;

    void Update()
    {
        // Verifica se a posição Y do inimigo é menor que o limite
        if (transform.position.x > xLimit)
        {
            // Desativa o GameObject
            Destroy(gameObject);
        }
    }

    // Este método é chamado quando outro collider 2D entra no trigger
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
