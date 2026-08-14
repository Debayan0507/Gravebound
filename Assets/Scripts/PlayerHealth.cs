using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI Integration")]
    public Slider healthSlider;

    private PlayerMovement playerMovement;

    void Start()
    {
        currentHealth = maxHealth;

        // Grab the movement script to check for the shield state
        playerMovement = GetComponent<PlayerMovement>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        // 30% Damage Reduction if the shield is active
        if (playerMovement != null && playerMovement.isShielding)
        {
            damage = Mathf.RoundToInt(damage * 0.7f);
            Debug.Log("Shield Blocked! Reduced damage taken.");
        }

        // Reduce current health by damage amount
        currentHealth -= damage;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // Check if health is zero or below
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Remove player from scene
        Destroy(gameObject);
    }
}