using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_1 : MonoBehaviour
{

    private Animator animator;
    
    
    public GameObject bullet,my_bullet;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        shoot();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void shoot(){
        my_bullet = Instantiate(bullet, transform.position - new Vector3(0.5f,-0.7f,0f), Quaternion.identity);
        my_bullet.gameObject.GetComponent<bullet>().setSpeed(-2f * transform.localScale.x);
        animator.SetBool("shoot",false);
        StartCoroutine(delayShoot());
    }

    public IEnumerator delayShoot(){

        yield return new WaitForSeconds(2f);
        animator.SetBool("shoot",true);
    }


    private void OnMouseDown() {
        animator.SetBool("isDead",true);
    }
}
