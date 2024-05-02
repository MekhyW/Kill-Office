using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class enemy_shooter : MonoBehaviour
{

    private Animator animator;
    private bool IsMovingRight = false;
    private bool IsMovingLeft = true;
    private bool IsShooting = false;
    private bool ShootIsReady = true;
    [SerializeField] private bool isKillable = true;
    private AudioSource audioSource;
    private CapsuleCollider2D collider;
    private Rigidbody2D rb;
    [SerializeField] private bool isShootable = true;
    public AudioClip metalSfx;


    private bool IsDead = false;
    public LayerMask ignoreLayer;


    public GameObject bullet;
    private GameObject my_bullet;
    // Start is called before the first frame update
    void Start()
    {
        //ignoreLayer = LayerMask.NameToLayer("Ignore Raycast");
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        collider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsDead)
        {
            return;
        }
        Walk();
        EnemySight();
    }

    public void shoot()
    {
        if (transform.localScale.x == -1)
        {
            my_bullet = Instantiate(bullet, transform.position - new Vector3(0.5f, -0.7f, 0f), Quaternion.identity);
        }
        else
        {
            my_bullet = Instantiate(bullet, transform.position - new Vector3(-0.5f, -0.7f, 0f), Quaternion.identity);
        }
        my_bullet.gameObject.GetComponent<bullet>().setSpeed(-2f * transform.localScale.x);
        animator.SetBool("shoot", false);
        IsShooting = false;
        StartCoroutine(delayShoot());
        audioSource.PlayOneShot(audioSource.clip, 0.5f);
    }

    public IEnumerator delayShoot()
    {

        yield return new WaitForSeconds(1f);
        ShootIsReady = true;
    }

    private void EnemySight()
    {
        Vector2 size = new Vector2(0.5f, 0.5f);
        Vector2 direction = Vector2.right * transform.localScale.x * size.x;
        Vector2 direction2 = Vector2.up * size.y * 2;
        Vector2 offset = new Vector2(0f, 0.5f);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + offset, direction, 10f, ~ignoreLayer);
        RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position + offset, direction2, 10f, ~ignoreLayer);
        if (hit.collider != null)
        {
            //print(hit.collider.gameObject.tag);
        }
        if (hit2.collider != null)
        {
            //print(hit2.collider.gameObject.tag);
        }
        // draw a line to see the raycast
        Debug.DrawRay((Vector2)transform.position + offset, direction, Color.green);
        Debug.DrawRay((Vector2)transform.position + offset, direction2, Color.red);
        if (hit.collider != null && hit.collider.gameObject.tag == "Player")
        {
            if (ShootIsReady)
            {
                ShootIsReady = false;
                animator.SetBool("shoot", true);
                IsShooting = true;
            }
            else
            {
                //print("not ready to shoot");
            }
        }
        if (hit2.collider != null && hit2.collider.gameObject.tag == "Player")
        {
            //print("player is above");
            if (IsMovingLeft)
            {
                transform.localScale = new Vector3(1f, 1f, 1f); // flip the sprite
                IsMovingRight = true;
                IsMovingLeft = false;
            }
            else if (IsMovingRight)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f); // flip the sprite
                IsMovingRight = false;
                IsMovingLeft = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemyWaypointA")
        {
            //print("hit waypoint A");
            transform.localScale = new Vector3(1f, 1f, 1f); // flip the sprite
            IsMovingRight = true;
            IsMovingLeft = false;
        }
        else if (collision.gameObject.tag == "EnemyWaypointB")
        {
            //print("hit waypoint B");
            transform.localScale = new Vector3(-1f, 1f, 1f); // flip the sprite
            IsMovingRight = false;
            IsMovingLeft = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //print(collision.gameObject.tag);

        // Essa daqui ainda não está funcionando!!!!!

        if (collision.gameObject.tag == "wall")
        {
            //print("hit wall");
            if (IsMovingRight)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f); //   flip the sprite
                IsMovingRight = false;
                IsMovingLeft = true;
            }
            else if (IsMovingLeft)
            {
                transform.localScale = new Vector3(1f, 1f, 1f); // flip the sprite
                IsMovingRight = true;
                IsMovingLeft = false;
            }
        }
    }

    private void Walk()
    {
        if (IsShooting)
        {
            return;
        }
        else if (IsMovingRight)
        {
            transform.Translate(Vector2.right * 1f * Time.deltaTime);
        }
        else if (IsMovingLeft)
        {
            transform.Translate(Vector2.left * 1f * Time.deltaTime);
        }
    }


    private void OnMouseDown()
    {
        if (isShootable)
        {
            if (isKillable)
            {
                die();
            }
            else
            {
                stun();
            }
        }
        else{
            audioSource.PlayOneShot(metalSfx,1.5f);
        }
    }

    public void stun()
    {
        animator.SetBool("isDead", true);
        IsDead = true;
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        StartCoroutine(revive());
    }

    public IEnumerator revive()
    {
        yield return new WaitForSeconds(3f);
        IsDead = false;
        animator.SetBool("isDead", false);
        collider.enabled = true;
        rb.simulated = true;
    }

    public void die()
    {
        animator.SetBool("isDead", true);
        IsDead = true;
        // disable the collider
        collider.enabled = false;
        // disable the rigidbody
        rb.simulated = false;

    }
}
