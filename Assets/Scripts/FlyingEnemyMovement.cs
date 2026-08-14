using UnityEngine;
using System.Collections;

public class FlyingEnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float flightAltitude = 4f;
    public float changeStateTime = 2f;
    public float deathYLevel = -5f;

    [Header("Detection & Standard Attack")]
    [Tooltip("Type the exact detection range in this box.")]
    public float detectionRadius = 12f;
    public float chaseSpeed = 5f;
    public float swoopSpeed = 8f;
    public float swoopTriggerRange = 1.5f;
    public float attackCooldown = 2f;
    public int attackDamage = 2;

    [Header("Kamikaze Settings")]
    public float kamikazeRange = 4.5f;
    public float kamikazeSpeed = 12f;

    [Tooltip("Use this slider to set the exact angle the crow dives at (in degrees).")]
    [Range(15f, 85f)]
    public float kamikazeDiveAngle = 45f;

    public int kamikazeDamage = 6;
    public GameObject explosionPrefab;
    public float explosionRadius = 4f;

    [Header("Hover Settings")]
    public float hoverSpeed = 3f;
    public float hoverHeight = 0.5f;

    private Rigidbody2D rb;
    private float moveDirection;
    private float timer;
    private float attackTimer;
    private float kamikazeTimer;
    private Transform player;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        timer = Random.Range(0f, changeStateTime);
        PickNewState();
    }

    void Update()
    {
        if (transform.position.y <= deathYLevel)
        {
            Destroy(gameObject);
            return;
        }

        if (isAttacking) return;

        attackTimer -= Time.deltaTime;
        kamikazeTimer -= Time.deltaTime;

        if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRadius)
        {
            float distX = Mathf.Abs(player.position.x - transform.position.x);

            if (distX <= swoopTriggerRange)
            {
                if (attackTimer <= 0f)
                {
                    StartCoroutine(SwoopAttackRoutine());
                    attackTimer = attackCooldown;
                }
            }
            else if (distX <= kamikazeRange)
            {
                if (kamikazeTimer <= 0f)
                {
                    if (Random.Range(0, 5) == 0)
                    {
                        StartCoroutine(KamikazeRoutine());
                    }
                    else
                    {
                        kamikazeTimer = 1f;
                    }
                }

                if (!isAttacking)
                {
                    ChasePlayer();
                }
            }
            else
            {
                if (!isAttacking)
                {
                    ChasePlayer();
                }
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
        if (isAttacking) return;

        float targetY = (player != null ? player.position.y : 0f) + flightAltitude;
        float verticalHover = Mathf.Cos(Time.time * hoverSpeed) * hoverHeight;

        float velocityY = (targetY - transform.position.y) * 2f + verticalHover;

        bool isChasing = (player != null && Vector2.Distance(transform.position, player.position) <= detectionRadius);
        float currentSpeed = isChasing ? chaseSpeed : moveSpeed;

        rb.linearVelocity = new Vector2(moveDirection * currentSpeed, velocityY);
    }

    void ChasePlayer()
    {
        float dirX = Mathf.Sign(player.position.x - transform.position.x);
        moveDirection = dirX;
        transform.rotation = Quaternion.Euler(0, dirX > 0 ? 180f : 0f, 0);
    }

    private IEnumerator SwoopAttackRoutine()
    {
        isAttacking = true;

        float dirToPlayer = Mathf.Sign(player.position.x - transform.position.x);
        transform.rotation = Quaternion.Euler(0, dirToPlayer > 0 ? 180f : 0f, 0);

        Vector2 targetPos = player.position;
        Vector2 diveDir = (targetPos - (Vector2)transform.position).normalized;

        rb.linearVelocity = diveDir * swoopSpeed;

        float swoopTime = 0f;
        while (swoopTime < 1f)
        {
            swoopTime += Time.deltaTime;

            if (Vector2.Distance(transform.position, player.position) <= 1.2f)
            {
                player.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
                player.SendMessage("ApplyKnockback", dirToPlayer, SendMessageOptions.DontRequireReceiver);
                break;
            }
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isAttacking = false;
    }

    private IEnumerator KamikazeRoutine()
    {
        isAttacking = true;

        float dirToPlayer = Mathf.Sign(player.position.x - transform.position.x);
        transform.rotation = Quaternion.Euler(0, dirToPlayer > 0 ? 180f : 0f, 0);

        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.4f);

        // Convert the slider angle into radians for the math calculation
        float angleRad = kamikazeDiveAngle * Mathf.Deg2Rad;

        // Calculate the exact trajectory using the slider's angle
        Vector2 diveDirection = new Vector2(Mathf.Cos(angleRad) * dirToPlayer, -Mathf.Sin(angleRad)).normalized;

        rb.linearVelocity = diveDirection * kamikazeSpeed;

        float diveTime = 0f;
        bool hasExploded = false;

        // Ground is slightly below the player
        float groundYLevel = player.position.y - 1.2f;

        while (diveTime < 1.5f)
        {
            diveTime += Time.deltaTime;

            // Explode the exact moment it crosses the ground level
            if (transform.position.y <= groundYLevel)
            {
                Explode(transform.position, dirToPlayer);
                hasExploded = true;
                break;
            }

            yield return null;
        }

        if (!hasExploded)
        {
            Explode(transform.position, dirToPlayer);
        }
    }

    void Explode(Vector2 explosionPos, float pushDirection)
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, explosionPos, Quaternion.identity);
        }

        if (player != null && Vector2.Distance(explosionPos, player.position) <= explosionRadius)
        {
            player.SendMessage("TakeDamage", kamikazeDamage, SendMessageOptions.DontRequireReceiver);
            player.SendMessage("ApplyKnockback", pushDirection, SendMessageOptions.DontRequireReceiver);
        }

        Destroy(gameObject);
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