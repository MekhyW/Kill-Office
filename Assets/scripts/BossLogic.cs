using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using static MBT.Blackboard;


public class BossLogic : MonoBehaviour
{
    private Animator animator;
    private CapsuleCollider2D collider;
    private Rigidbody2D rb;
    private AudioSource audioSource;


    public GameObject fase1;
    public GameObject fase2;
    public GameObject fase3;

    private MBT.Blackboard blackboard;

    public AudioClip metalSfx;
    private bool IsDead = false;



    public int health = 20;

    [SerializeField] private bool isShootable = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        collider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();


        
    }

        private void OnMouseDown()
    {
        if (isShootable)
        {
            health -= 1;
            Debug.Log("essa é a vida do cara");
            Debug.Log(health);
            if (health == 0)
            {
                die();
            }
            else
            {
                dmg();
            }
            if (health < 10) {
                Debug.Log("FASE3");
                fase2.SetActive(false);
                fase3.SetActive(true);
            }
            else if (health< 15){
                Debug.Log("FASE2");
                fase1.SetActive(false);
                fase2.SetActive(true);
            }
        }
        else{
            audioSource.PlayOneShot(metalSfx,1.5f);
        }
    }

        public void die()
    {
        animator.SetBool("Died", true);
        IsDead = true;
        // disable the collider
        collider.enabled = false;
        // disable the rigidbody
        rb.simulated = false;

    }

        public void dmg()
    {
        animator.SetTrigger("damageBoss");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
