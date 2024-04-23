using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_1 : MonoBehaviour
{

    
    
    public GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void shoot(){
        Instantiate(bullet, transform.position - new Vector3(0.5f,-0.7f,0f), Quaternion.identity);
    }


    private void OnMouseDown() {
        Debug.Log("MOUSEDOWN");
    }
}
