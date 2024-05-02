using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundCheck : MonoBehaviour
{

    private bool isGrounded;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GroundCheck(){
        return isGrounded;
    }

    void OnCollisionStay2D(Collision2D other){
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Barrel")){
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D obj){
        isGrounded = false;
    }
}
