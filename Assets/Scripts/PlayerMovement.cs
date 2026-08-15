using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Required to use HashSet for enemy tracking

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public float horizontalInput;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Dash Attack Settings")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 5f; // Increased cooldown to 5 seconds
    public int totalDashDamage = 20;
    public LayerMask enemyLayer;

    private bool isDashing;
    private bool canDash = true;
    private bool isKnockedBack = false;

    public bool isGrounded = true;

    [Header("Shield Settings")]
    public bool isShielding = false;
    public float shieldDuration = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Stop Move() from overriding the dash
        if (isDashing || isKnockedBack) return;

        Jump();
        Move();
        Dash();
        Shield();
    }

    void Shield()
    {
        // Triggers the shield block window when right-clicking (Mouse1)
        if (Input.GetKeyDown(KeyCode.Mouse1) && !isShielding)
        {
            StartCoroutine(ShieldRoutine());
        }
    }

    private IEnumerator ShieldRoutine()
    {
        isShielding = true;

        // The player has a 0.5 second window to perfectly block the attack
        yield return new WaitForSeconds(shieldDuration);

        isShielding = false;
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

        // Project a BoxCast forward to catch everything inside the dash path
        Vector2 boxSize = GetComponent<Collider2D>().bounds.size;
        float dashDistance = dashSpeed * dashDuration;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, boxSize, 0f, new Vector2(dashDirection, 0f), dashDistance, enemyLayer);

        if (hits.Length > 0)
        {
            // HashSet ensures we only count each enemy once, even if they have multiple colliders
            HashSet<GameObject> uniqueEnemies = new HashSet<GameObject>();
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.GetComponent<EnemyHealth>() != null)
                {
                    uniqueEnemies.Add(hit.collider.gameObject);
                }
            }

            if (uniqueEnemies.Count > 0)
            {
                // Split the damage based on how many ghosts are caught in the dash
                int damagePerEnemy = Mathf.RoundToInt((float)totalDashDamage / uniqueEnemies.Count);

                foreach (GameObject enemy in uniqueEnemies)
                {
                    enemy.GetComponent<EnemyHealth>().TakeDamage(damagePerEnemy);
                }
            }
        }

        yield return new WaitForSeconds(dashDuration);

        playerRb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.C) && canDash)
        {
            StartCoroutine(DashRoutine());
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

        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    public void ApplyKnockback(float dirX)
    {
        StartCoroutine(KnockbackRoutine(dirX));
    }

    private IEnumerator KnockbackRoutine(float dirX)
    {
        isKnockedBack = true;
        playerRb.linearVelocity = Vector2.zero;

        playerRb.AddForce(new Vector2(dirX * 10f, 5f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.3f);

        isKnockedBack = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}