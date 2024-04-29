using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class enemy_1 : MonoBehaviour
{

    private Animator animator;
    private bool IsMovingRight = false;
    private bool IsMovingLeft = true;
    private bool IsShooting = false;
    private bool ShootIsReady = true;

    private bool IsDead = false;
    
    
    public GameObject bullet;
    private GameObject my_bullet;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate(){
        if (IsDead){
            return;
        }
        Walk();
        EnemySight();
    }

    public void shoot(){
        my_bullet = Instantiate(bullet, transform.position - new Vector3(0.5f,-0.7f,0f), Quaternion.identity); 
        my_bullet.gameObject.GetComponent<bullet>().setSpeed(-2f * transform.localScale.x);
        animator.SetBool("shoot",false);
        IsShooting = false;
        StartCoroutine(delayShoot()); 
    }

    public IEnumerator delayShoot(){

        yield return new WaitForSeconds(1f);
        ShootIsReady = true;
    }

    private void EnemySight(){
        Vector2 size = new Vector2(0.5f,0.5f);
        Vector2 direction = Vector2.right * transform.localScale.x * size.x;
        Vector2 offset = new Vector2(0f,0.5f);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + offset, direction, 10f);
        RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position + offset, Vector2.up, 10f);
        if (hit.collider != null){
            print(hit.collider.gameObject.tag);
        }
        // draw a line to see the raycast
        Debug.DrawRay((Vector2)transform.position + offset, direction, Color.green);
        Debug.DrawRay((Vector2)transform.position + offset, Vector2.up, Color.red);
        if (hit.collider != null && hit.collider.gameObject.tag == "Player")
        {
            if (ShootIsReady){
                ShootIsReady = false;
                animator.SetBool("shoot",true);
                IsShooting = true;
            }
            else{
                print("not ready to shoot");
            }
        }
        if(hit2.collider != null && hit2.collider.gameObject.tag == "Player"){
                if (IsMovingLeft){
                transform.localScale = new Vector3(1f,1f,1f); // flip the sprite
                IsMovingRight = true;
                IsMovingLeft = false;
            }
            else if(IsMovingRight){
                transform.localScale = new Vector3(-1f,1f,1f); // flip the sprite
                IsMovingRight = false;
                IsMovingLeft = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "EnemyWaypointA"){
            //print("hit waypoint A");
            transform.localScale = new Vector3(1f,1f,1f); // flip the sprite
            IsMovingRight = true;
            IsMovingLeft = false;
        }
        else if(collision.gameObject.tag == "EnemyWaypointB"){
            //print("hit waypoint B");
            transform.localScale = new Vector3(-1f,1f,1f); // flip the sprite
            IsMovingRight = false;
            IsMovingLeft = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        //print(collision.gameObject.tag);

        // Essa daqui ainda não está funcionando!!!!!

        if(collision.gameObject.tag == "wall"){
            //print("hit wall");
            if(IsMovingRight){
                transform.localScale = new Vector3(-1f,1f,1f); //   flip the sprite
                IsMovingRight = false;
                IsMovingLeft = true;
            }else if(IsMovingLeft){
                transform.localScale = new Vector3(1f,1f,1f); // flip the sprite
                IsMovingRight = true;
                IsMovingLeft = false;
            }
        }
    }

    private void Walk(){
        if (IsShooting){
            return;
        }
        else if(IsMovingRight){
            transform.Translate(Vector2.right * 1f * Time.deltaTime);
        }else if(IsMovingLeft){
            transform.Translate(Vector2.left * 1f * Time.deltaTime);
        }
    }


    private void OnMouseDown() {
        die();
    }

    public void die(){
        animator.SetBool("isDead",true);
        IsDead = true;
        // disable the collider
        GetComponent<CapsuleCollider2D>().enabled = false;
        // disable the rigidbody
        GetComponent<Rigidbody2D>().simulated = false;

    }
}
