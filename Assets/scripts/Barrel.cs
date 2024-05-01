using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public GameObject explosionRadius;
    private float radius = 3f;
    private Animator animator;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        explosionRadius.SetActive(false);
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnMouseDown()
    {
        explosionRadius.SetActive(true);
        StartCoroutine(explode());
        
    }

    public IEnumerator explode(){
        audioSource.PlayOneShot(audioSource.clip,1f);
        animator.SetBool("explode",true);
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
