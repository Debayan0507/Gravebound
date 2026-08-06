using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float changeStateTime = 2f;
    public float deathYLevel = -3f;

    [Header("Attack Settings")]
    public float detectionRadius = 2f;
    public float dashForce = 50f;
    public float dashCooldown = 2f;

    private Rigidbody2D rb;
    private float moveDirection;
    private float timer;
    private float attackTimer;
    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;

        // Randomizes the starting timer so they don't move in perfect sync
        timer = Random.Range(0f, changeStateTime);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        PickNewState();
    }

    void Update()
    {
        if (transform.position.y <= deathYLevel)
        {
            Destroy(gameObject);
            return;
        }

        attackTimer -= Time.deltaTime;

        if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRadius)
        {
            if (attackTimer <= 0f)
            {
                DashAttack();
                attackTimer = dashCooldown;
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= changeStateTime)
            {
                PickNewState();
                timer = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
    }

    void DashAttack()
    {
        float dirX = Mathf.Sign(player.position.x - transform.position.x);
        moveDirection = dirX;

        // 0 degrees for right, 180 degrees for left
        transform.rotation = Quaternion.Euler(0, dirX > 0 ? 180f : 0f, 0);

        rb.AddForce(new Vector2(dashForce * dirX, 10f), ForceMode2D.Impulse);
    }

    void PickNewState()
    {
        int state = Random.Range(0, 3);
        if (state == 0)
        {
            moveDirection = 0f;
        }
        else if (state == 1)
        {
            moveDirection = 1f;
            transform.rotation = Quaternion.Euler(0, 0f, 0);
        }
        else if (state == 2)
        {
            moveDirection = -1f;
            transform.rotation = Quaternion.Euler(0, 180f, 0);
        }
    }
}