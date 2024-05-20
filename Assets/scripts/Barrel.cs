using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public GameObject explosionRadius;
    private float radius = 3f;
    private Animator animator;
    private AudioSource audioSource;
    public GameObject barrelPrefab;
    private bool waiting = false;

    // Start is called before the first frame update
    void Start()
    {
        explosionRadius.SetActive(false);
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnMouseDown()
    {
        StartCoroutine(explode());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !waiting)
        {
            print("Player collided with barrel");
            explosionRadius.GetComponent<ExplosionRadius>().isJump=true;
            StartCoroutine(explode());
            waiting = true;
        }
    }

    public IEnumerator explode(){
        explosionRadius.SetActive(true);
        audioSource.PlayOneShot(audioSource.clip,1f);
        animator.SetBool("explode",true);
        // desabilita o colisor do barril
        // make the object invisible
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        invokeRespawn();
    }

    private void invokeRespawn(){
        Invoke("respawn",2f);
    }

    private void respawn(){
        print("Respawning");
        //gameObject.SetActive(true);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
        animator.SetBool("explode",false);
        waiting = false;
        // retorna o objeto para o estado inicial

    }
}
