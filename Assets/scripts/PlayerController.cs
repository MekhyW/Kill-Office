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
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float coyoteTimeCounter;
    [SerializeField] private float jumpForce = 8.0f;
    private bool isGrounded;
    private Rigidbody2D rb;
    private InputAction movement;
    private SpriteRenderer spriteRenderer;
    private bool facingRight;
    private Animator anim;

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
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float movementValue = movement.ReadValue<float>();
        if (movementValue != 0){
            anim.SetBool("isRunning", true);
            if (movementValue > 0)
            {
                spriteRenderer.flipX = false;
                facingRight = true;
            }
            else if (movementValue < 0)
            {
                spriteRenderer.flipX = true;
                facingRight = false;
            }
        }
        else{
            anim.SetBool("isRunning", false);
        }

    }

    void OnJump()
    {
        if (coyoteTimeCounter > 0)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("grounded", true);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            anim.SetBool("grounded", false);
        }
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
            anim.SetBool("goingUp", false);
        }
        else
        {
            rb.gravityScale = gravityScale;
            anim.SetBool("goingUp", true);
        }

    }
    public void OnReset(){
        Debug.Log("RESET");
        UnityEngine.SceneManagement.SceneManager.LoadScene("industrial_level");
    }
}
