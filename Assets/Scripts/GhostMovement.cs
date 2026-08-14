using UnityEngine;
using System.Collections;

public class GhostMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float changeStateTime = 2f;
    public float deathYLevel = -3f;

    [Header("Detection & Dash Settings")]
    [Tooltip("Type the exact detection range in this box.")]
    public float detectionRadius = 4f;
    public float dashCooldown = 5f;

    [Header("Melee Attack Settings")]
    public float meleeRadius = 1.2f;
    public float meleeCooldown = 2f;
    public int attackDamage = 5;

    private Rigidbody2D rb;
    private float moveDirection;
    private float timer;
    private float attackTimer;
    private float meleeTimer;
    private Transform player;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
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

        if (isAttacking) return;

        attackTimer -= Time.deltaTime;
        meleeTimer -= Time.deltaTime;

        float dist = player != null ? Vector2.Distance(transform.position, player.position) : float.MaxValue;
        float tenPercentRange = detectionRadius * 0.1f;

        if (dist <= tenPercentRange)
        {
            if (meleeTimer <= 0f)
            {
                StartCoroutine(SuperDashAttackRoutine());
                meleeTimer = meleeCooldown;
            }
            else
            {
                ChasePlayer();
            }
        }
        else if (dist <= meleeRadius)
        {
            if (meleeTimer <= 0f)
            {
                MeleeAttack();
                meleeTimer = meleeCooldown;
            }
            else
            {
                ChasePlayer();
            }
        }
        else if (dist <= detectionRadius)
        {
            if (attackTimer <= 0f)
            {
                if (Random.Range(0, 5) == 0)
                {
                    StartCoroutine(JumpAttackRoutine());
                    attackTimer = dashCooldown;
                }
                else
                {
                    attackTimer = 1f;
                }
            }

            if (!isAttacking)
            {
                ChasePlayer();
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

        bool isChasing = (player != null && Vector2.Distance(transform.position, player.position) <= detectionRadius);
        float currentSpeed = isChasing ? chaseSpeed : moveSpeed;

        rb.linearVelocity = new Vector2(moveDirection * currentSpeed, rb.linearVelocity.y);
    }

    void ChasePlayer()
    {
        float dirX = Mathf.Sign(player.position.x - transform.position.x);
        moveDirection = dirX;
        transform.rotation = Quaternion.Euler(0, dirX > 0 ? 0f : 180f, 0);
    }

    void MeleeAttack()
    {
        float dirToPlayer = Mathf.Sign(player.position.x - transform.position.x);
        transform.rotation = Quaternion.Euler(0, dirToPlayer > 0 ? 0f : 180f, 0);

        player.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
        player.SendMessage("ApplyKnockback", dirToPlayer, SendMessageOptions.DontRequireReceiver);
    }

    private IEnumerator SuperDashAttackRoutine()
    {
        isAttacking = true;
        float dirToPlayer = Mathf.Sign(player.position.x - transform.position.x);
        transform.rotation = Quaternion.Euler(0, dirToPlayer > 0 ? 0f : 180f, 0);

        rb.linearVelocity = new Vector2(dirToPlayer * (moveSpeed * 2f), rb.linearVelocity.y);

        yield return new WaitForSeconds(0.2f);
        rb.linearVelocity = Vector2.zero;

        if (player != null && Vector2.Distance(transform.position, player.position) <= meleeRadius)
        {
            player.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
            player.SendMessage("ApplyKnockback", dirToPlayer, SendMessageOptions.DontRequireReceiver);
        }

        isAttacking = false;
    }

    private IEnumerator JumpAttackRoutine()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dirToPlayer = Mathf.Sign(player.position.x - transform.position.x);
        Vector2 startPos = transform.position;
        Vector2 targetPos = new Vector2(player.position.x + (dirToPlayer * 0.5f), transform.position.y);

        float jumpDuration = 0.5f;
        float elapsed = 0f;

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;

            float currentX = Mathf.Lerp(startPos.x, targetPos.x, t);
            float currentY = Mathf.Lerp(startPos.y, targetPos.y, t) + Mathf.Sin(t * Mathf.PI) * 2f;

            transform.position = new Vector2(currentX, currentY);
            yield return null;
        }

        dirToPlayer = Mathf.Sign(player.position.x - transform.position.x);
        transform.rotation = Quaternion.Euler(0, dirToPlayer > 0 ? 0f : 180f, 0);
        rb.gravityScale = originalGravity;

        if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRadius * 1.5f)
        {
            player.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
        }

        isAttacking = false;
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