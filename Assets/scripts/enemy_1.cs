using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_1 : MonoBehaviour
{

    private Animator animator;
    
    
    public GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void shoot(){
        Instantiate(bullet, transform.position - new Vector3(0.5f,-0.7f,0f), Quaternion.identity);
    }


    private void OnMouseDown() {
        animator.SetBool("isDead",true);
    }
}
