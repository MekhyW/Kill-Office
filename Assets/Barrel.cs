using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public GameObject explosionRadius;
    private float radius = 3f;

    // Start is called before the first frame update
    void Start()
    {
        explosionRadius.SetActive(false);
    }

    void OnMouseDown()
    {
        explosionRadius.SetActive(true);
        gameObject.SetActive(false);
    }
}
