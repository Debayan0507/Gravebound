using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public float horizontalInput;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private bool isDashing;
    private bool canDash = true;

    public bool isGrounded = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Stop Move() from overriding the dash
        if (isDashing) return;

        Jump();
        Move();

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = playerRb.gravityScale;
        playerRb.gravityScale = 0f;

        // Determine dash direction based on current input
        float dashDirection = horizontalInput != 0 ? Mathf.Sign(horizontalInput) : 1f;
        playerRb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        playerRb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.W))
        {
            playerRb.AddForceY(jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    void Move()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        playerRb.linearVelocity = new Vector2(horizontalInput * moveSpeed, playerRb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}