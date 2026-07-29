using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    [Header("Movement Settings")]

    // The movement speed of the ghost.
    public float moveSpeed = 2f;

    // Time in seconds before the ghost picks a new random direction.
    public float directionChangeTime = 3f;

    private Rigidbody2D rb;
    private Vector2 currentDirection;
    private float timer;

    void Start()
    {
        // Grabs the Rigidbody2D component automatically when the ghost spawns.
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
        // Applies the movement velocity directly to the physics body.
        rb.linearVelocity = currentDirection * moveSpeed;
    }

    void PickRandomDirection()
    {
        // Picks a random normalized 2D direction for the ghost to drift towards.
        currentDirection = Random.insideUnitCircle.normalized;
    }
}