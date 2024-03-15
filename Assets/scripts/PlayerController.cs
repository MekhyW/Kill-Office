using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float gravityScale = 2.0f;
    [SerializeField] private float fallGravityMultiplier = 2.0f;
    [SerializeField] private float velPower = 2.0f;
    [SerializeField] private float deccel = 2.0f;
    [SerializeField] private float accel = 2.0f;
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float coyoteTime = 2.0f;
    [SerializeField] private float coyoteTimeCounter = 2.0f;
    private bool isGrounded;
    private Rigidbody2D rb;
    private InputAction movement;
    private SpriteRenderer spriteRenderer;
    private bool facingRight;

    public PlayerInputActions playerControls;
    // Start is called before the first frame update

    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        movement = playerControls.Player.Move;
        movement.Enable();
    }

    private void OnDisable()
    {
        movement.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movement.ReadValue<float>() > 0)
        {
            spriteRenderer.flipX = false;
            facingRight = true;
        }
        else if (movement.ReadValue<float>() < 0)
        {
            spriteRenderer.flipX = true;
            facingRight = false;
        }

    }

    void OnJump(){
        Debug.Log("JUMP");
    }

    public void FixedUpdate()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        float move = movement.ReadValue<float>();
        float targetSpeed = move * speed;
        float speedDiff = targetSpeed - rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accel : deccel;
        float movementFactor = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);

        rb.AddForce(movementFactor * Vector2.right);

        if (rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }

    }
}
