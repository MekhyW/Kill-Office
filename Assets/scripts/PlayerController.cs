using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{

    private float gravityScale = 2.0f;
    private float fallGravityMultiplier = 3.0f;
    private float velPower = 2.0f;
    private float deccel = 2.0f;
    private float accel = 2.0f;
    private float speed = 10.0f;
    private float coyoteTime = 0.1f;
    private float coyoteTimeCounter;
    private float jumpForce = 14.0f;
    private float jumpCutMultiplier = 0.9f;
    private float wallJumpBounce = 12.0f;
    private bool isGrounded;
    private bool isWallJumping = false;
    private float walljumpTime = 0.5f;
    private Rigidbody2D rb;
    private InputAction movement, jump;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;
    private Animator anim;
    private AudioSource audioSource;
    private bool canDash = true;
    private bool isDashing;
    private float dashingTime = 0.3f;
    private float dashingCooldown = 0.3f;
    private float dashingPower = 15f;
    private bool isWalled = false;
    private bool isWallSliding = false;
    private float wallSlidingSpeed = 2f;
    private Vector3 contactPoint;
    private Vector3 groundPosition;
    private int cardCount = 0;
    private bool isDead = false;

    public GameObject loseTextObject;
    public GameObject groundChecker;
    public AudioClip cardSfx;
    public AudioClip gameOverSfx;
    public AudioClip jumpSfx;
    public AudioClip dashSfx;
    public AudioClip shotSfx;
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = new Vector2(100,100);
    public GameObject manager;


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
        loseTextObject.SetActive(false);
        
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
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead){ 
            return;
        }

        isGrounded = groundChecker.GetComponent<groundCheck>().GroundCheck();// grabs ground check bool function from child
        if (isGrounded){
            anim.SetBool("grounded", true);
        }
        else{
            anim.SetBool("grounded", false);
        }

        if (transform.position.y < -8)
        {
            killPlayer();
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
        //Debug.Log(groundPosition.y);
        if (isDead){
            return;
        }

        if (isWallSliding)
        {
            CancelInvoke(nameof(stopWallJumping));
            isWallJumping = true;
            audioSource.PlayOneShot(jumpSfx,0.7f);
            spriteRenderer.flipX = !spriteRenderer.flipX;
            facingRight = !facingRight;
            rb.velocity = new Vector2(0f, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            if (contactPoint.x - transform.position.x > 0)
            {
                rb.AddForce(Vector2.left * wallJumpBounce, ForceMode2D.Impulse);
                Debug.Log("WJRight");
            }
            else
            {
                rb.AddForce(Vector2.right * wallJumpBounce, ForceMode2D.Impulse);
                Debug.Log("WJLeft");
            }
            Invoke(nameof(stopWallJumping), walljumpTime);
        }

        else if (coyoteTimeCounter > 0)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            audioSource.PlayOneShot(jumpSfx,0.7f);
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
        if (other.gameObject.CompareTag("Walls"))
        {
            //CancelInvoke(nameof(stopWallJumping));
            isWalled = true;
            //isWallJumping = false;
            // get the point of contact
            contactPoint = other.contacts[0].point;
            Debug.Log("ContactPoint: " + contactPoint);
        }
    }


    private void OnCollisionExit2D(Collision2D other)
    {

        if (other.gameObject.CompareTag("Walls"))
        {
            isWalled = false;
        }
    }

    public void FixedUpdate()
    {
        if (isDead){
            return;
        }
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

        // https://www.youtube.com/watch?v=KbtcEVCM7bw&list=PLdciXljwmrpFK5VV2qCiYt48E8IQzQko5&index=5
        if (!isWallJumping)
        {
            rb.AddForce(movementFactor * Vector2.right);
        }

        if (rb.velocity.y < 0 && !isDashing)
        {
            rb.gravityScale = gravityScale * fallGravityMultiplier;
            anim.SetBool("goingUp", false);
        }
        else if (!isDashing)
        {
            rb.gravityScale = gravityScale;
            anim.SetBool("goingUp", true);
        }

    }
    public void OnReset()
    {
        Debug.Log("RESET");
        manager.GetComponent<Manager>().restartLevel();
    }

    public void OnDash()
    {
        if (canDash)
        {
            audioSource.PlayOneShot(dashSfx,1f);
            StartCoroutine(Dash());
        }
    }

    public IEnumerator Dash()
    {
        Debug.Log("DASH");
        canDash = false;
        isDashing = true;
        anim.SetBool("isDashing", true);
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        if (facingRight)
        {
            rb.velocity = new Vector2(dashingPower, 0f);
        }
        else
        {
            rb.velocity = new Vector2(-dashingPower, 0f);
        }
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        anim.SetBool("isDashing", false);
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    void stopWallJumping()
    {
        isWallJumping = false;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Card")){
            cardCount++;
            other.gameObject.SetActive(false);
            audioSource.PlayOneShot(cardSfx,0.7f);
        }

        else if (other.gameObject.CompareTag("Spikes")){
            Debug.Log("SPIKES");
            anim.SetBool("isDead", true);
            killPlayer();
        }

        else if (other.gameObject.CompareTag("Bullet")){
            Destroy(other.gameObject);
            killPlayer();
        }

    }

    public void killPlayer(){
        isDead=true;
        anim.SetBool("isDead", true);
        StartCoroutine(OnDeath());
    }

    public IEnumerator OnDeath(){
        loseTextObject.SetActive(true);
        audioSource.PlayOneShot(gameOverSfx,0.7f);
        //play sfx
        yield return new WaitForSeconds(2f);
        OnReset();

    }


    public void OnFire(){
        audioSource.PlayOneShot(shotSfx,0.7f);

    }


}
