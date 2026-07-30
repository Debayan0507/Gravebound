using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float directionChangeTime = 3f;

    private Rigidbody2D rb;
    private Vector2 currentDirection;
    private float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PickRandomDirection();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= directionChangeTime)
        {
            PickRandomDirection();
            timer = 0f;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = currentDirection * moveSpeed;
    }

    void PickRandomDirection()
    {
        // Randomly choose left (-1) or right (1) on the X-axis
        float randomX = Random.value > 0.5f ? 1f : -1f;
        currentDirection = new Vector2(randomX, 0f);
    }
}