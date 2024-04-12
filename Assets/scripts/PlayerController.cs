using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{

    [SerializeField] private float gravityScale = 2.0f;
    [SerializeField] private float fallGravityMultiplier = 2.0f;
    [SerializeField] private float velPower = 2.0f;
    [SerializeField] private float deccel = 2.0f;
    [SerializeField] private float accel = 2.0f;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float coyoteTimeCounter;
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    private float wallJumpBounce = 5.0f;
    private bool isGrounded;
    private bool isWallJumping= false;
    private float walljumpTime = 0.3f ;
    private Rigidbody2D rb;
    private InputAction movement, jump;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;
    private Animator anim;
    private AudioSource audioSource;
    private bool canDash = true;
    private bool isDashing;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;
    private float dashingPower = 30f;
    private bool isWalled = false;
    private bool isWallSliding = false;  
    private float wallSlidingSpeed = 2f;   


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
        jump = playerControls.Player.Jump;
        jump.Enable();
        audioSource = GetComponent<AudioSource>();
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
        if (transform.position.y < -8)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single); // reload current scene
        }

        if (isDashing)
        {
            return;
        }

        float movementValue = movement.ReadValue<float>();
        if (movementValue != 0 && !isWallJumping)
        {
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
        else
        {
            anim.SetBool("isRunning", false);
        }

        if (!isGrounded && jump.ReadValue<float>() == 0 && rb.velocity.y > 0)
        { // jump while not holding button
            rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
        }

        WallSlide();

    }

    void OnJump()
    {
        if (coyoteTimeCounter > 0)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else if (isWallSliding)
        {
            isWallJumping = true;
            
            spriteRenderer.flipX = ! spriteRenderer.flipX;
            rb.velocity = new Vector2(0f, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            if (facingRight)
            {
                rb.AddForce(Vector2.left * wallJumpBounce, ForceMode2D.Impulse);
                Debug.Log("WJRight");
            }
            else
            {
                rb.AddForce(Vector2.right * wallJumpBounce, ForceMode2D.Impulse);
                Debug.Log("WJLeft");
            }
            Invoke(nameof(stopWallJumping),walljumpTime);
        }
    }

    private void WallSlide()
    {
        if (isWalled && !isGrounded && movement.ReadValue<float>() != 0f)
        {
            isWallSliding = true;
            anim.SetBool("isWallSliding", true);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
            anim.SetBool("isWallSliding", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("grounded", true);
        }
        if (other.gameObject.CompareTag("Walls"))
        {
            isWalled = true;
        }
    }


    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            anim.SetBool("grounded", false);
        }
        if (other.gameObject.CompareTag("Walls"))
        {
            isWalled = false;
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


        if (!isWallJumping){
            rb.AddForce(movementFactor * Vector2.right);
        }

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
    public void OnReset()
    {
        Debug.Log("RESET");
        UnityEngine.SceneManagement.SceneManager.LoadScene("industrial_level");
    }

    public void OnDash()
    {
        if (canDash)
        {
            StartCoroutine(Dash());
        }
    }

    public IEnumerator Dash()
    {
        Debug.Log("DASH");
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        if (facingRight){
            rb.velocity = (new Vector2(-1 * dashingPower, 0f));
        }
        else
        {
            rb.velocity = (new Vector2(1 * dashingPower, 0f));
        }
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    void stopWallJumping(){
        isWallJumping = false;
    }

}
