using System.Collections;
using UnityEngine;

public class ConstantMovement : MonoBehaviour
{
    private Vector3 movementDirection = Vector3.right;
    private float speed = 5.0f;
    private bool shouldMove = false;

    private void Start()
    {
        StartCoroutine(WaitAndMove());
    }

    public void SetMovement(Vector3 direction, float movementSpeed)
    {
        movementDirection = direction;
        speed = movementSpeed;
    }

    private IEnumerator WaitAndMove()
    {
        yield return new WaitForSeconds(1.0f); // Espera 1 segundo
        shouldMove = true;
    }

    private void Update()
    {
        if (shouldMove)
        {
            transform.Translate(movementDirection * speed * Time.deltaTime);
        }
    }
}
