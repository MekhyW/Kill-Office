using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disableObjects : MonoBehaviour
{   
    // Este método é chamado quando outro collider 2D entra no trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entrou no trigger");
        // Desativa o GameObject que entrou no trigger
        collision.gameObject.SetActive(false);
    }

}
