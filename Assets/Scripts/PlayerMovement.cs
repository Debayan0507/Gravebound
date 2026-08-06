using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public float horizontalInput;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float dashForce = 50f;

    public bool isGrounded = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        Move();
        Dash();
    }
    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            playerRb.AddForce(new Vector2(dashForce, 0), ForceMode2D.Impulse);
        }
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
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
