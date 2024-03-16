using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float _startingPos; //This is starting position of the sprites.
    private float _lengthOfSprite;    //This is the length of the sprites.
    public float AmountOfParallax;  //This is amount of parallax scroll. 
    public Camera MainCamera;   //Reference of the camera.
    // Start is called before the first frame update
    void Start()
    {
        _startingPos = transform.position.x;
        _lengthOfSprite = GetComponent<SpriteRenderer>().bounds.size.x;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Position = MainCamera.transform.position;
        float Temp = Position.x * (1 - AmountOfParallax);
        float Distance = Position.x * AmountOfParallax;

        Vector3 NewPosition = new Vector3(_startingPos + Distance, transform.position.y, transform.position.z);

        transform.position = NewPosition;

    }
}
