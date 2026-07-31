using UnityEngine;

public class FlyingEnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float changeStateTime = 2f;
    public float deathYLevel = -3f;

    // Only flying spirits will use this
    public float lifeTime = 10f;

    [Header("Hover Settings")]
    public float hoverSpeed = 2f;
    public float hoverHeight = 1f;

    private Rigidbody2D rb;
    private float moveDirection;
    private float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        PickNewState();

        // Destroys the flying spirit after 10 seconds
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (transform.position.y <= deathYLevel)
        {
            Destroy(gameObject);
            return;
        }

        timer += Time.deltaTime;

        if (timer >= changeStateTime)
        {
            PickNewState();
            timer = 0f;
        }
    }

    void FixedUpdate()
    {
        float verticalHover = Mathf.Cos(Time.time * hoverSpeed) * hoverHeight;
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, verticalHover);
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