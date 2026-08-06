using UnityEngine;

public class FlyingEnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float changeStateTime = 2f;
    public float deathYLevel = -3f;
    public float lifeTime = 10f;

    [Header("Attack Settings")]
    public float detectionRadius = 10f;
    public float chaseSpeed = 0.5f;

    [Header("Hover Settings")]
    public float hoverSpeed = 2f;
    public float hoverHeight = 1f;

    private Rigidbody2D rb;
    private float moveDirection;
    private float timer;
    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        PickNewState();
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (transform.position.y <= deathYLevel)
        {
            Destroy(gameObject);
            return;
        }

        if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRadius)
        {
            // Face the player and move towards them
            float dirX = Mathf.Sign(player.position.x - transform.position.x);
            moveDirection = dirX;
            transform.rotation = Quaternion.Euler(0, dirX > 0 ? 180f : 0f, 0);
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
        float verticalHover = Mathf.Cos(Time.time * hoverSpeed) * hoverHeight;

        // Dynamically apply slow speed if player is in range
        bool isChasing = (player != null && Vector2.Distance(transform.position, player.position) <= detectionRadius);
        float currentSpeed = isChasing ? chaseSpeed : moveSpeed;

        rb.linearVelocity = new Vector2(moveDirection * currentSpeed, verticalHover);
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
            transform.rotation = Quaternion.Euler(0, 180f, 0);
        }
        else if (state == 2)
        {
            moveDirection = -1f;
            transform.rotation = Quaternion.Euler(0, 0f, 0);
        }
    }
}