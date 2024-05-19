using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionRadius : MonoBehaviour
{
    void OnEnable(){
        Debug.Log("Comecou");
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay(){
        yield return new WaitForSeconds(0.3f);
        this.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        print(collider.gameObject.tag);
        if (collider.gameObject.CompareTag("Enemy"))
        {
            collider.gameObject.GetComponent<enemy_shooter>().die();
        }

        if (collider.gameObject.CompareTag("Player")){
            Vector3 diff = collider.gameObject.transform.position - transform.position;
            collider.gameObject.GetComponent<PlayerController>().Explode(diff.normalized*1000);
        }
        if (collider.gameObject.CompareTag("Barrel")){
            Debug.Log("CHAIN REACTION");
            collider.gameObject.GetComponent<Barrel>().explode();
        }
    }

}
