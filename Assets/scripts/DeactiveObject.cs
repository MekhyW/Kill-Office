using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactiveObject : MonoBehaviour
{
    // Variável para definir o limite da posição Y
    public float yLimit = 0f;

    void Update()
    {
        // Verifica se a posição Y do inimigo é menor que o limite
        if (transform.position.y < yLimit)
        {
            // Desativa o GameObject
            gameObject.SetActive(false);
        }
    }

}
