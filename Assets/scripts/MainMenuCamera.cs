using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    private bool invert = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.x >= 200){
            invert = true;
        }
        if (transform.position.x <= -200){
            invert = false;
        }


        if (invert)
        {
            transform.position += new Vector3(-0.1f, 0f, 0f);
        }
        else
        {
            transform.position += new Vector3(0.1f, 0f, 0f);
        }

    }


}
