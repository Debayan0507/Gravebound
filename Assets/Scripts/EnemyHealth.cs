using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        // Reduce current health by damage amount
        currentHealth -= damage;
        Debug.Log(name + " took " + damage + " damage!");

        // Check if health is zero or below
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(name + " died!");
        Destroy(gameObject); // Remove enemy from scene
    }
}
