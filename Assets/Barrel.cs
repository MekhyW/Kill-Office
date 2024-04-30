using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public GameObject explosionRadius;
    private float radius = 3f;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        explosionRadius.SetActive(false);
        animator = GetComponent<Animator>();
    }

    void OnMouseDown()
    {
        explosionRadius.SetActive(true);
        StartCoroutine(explode());
        
    }

    public IEnumerator explode(){
        animator.SetBool("explode",true);
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
