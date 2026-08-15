using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int attackDamage = 20;       // Damage dealt per attack
    public float attackRange = 3f;      // Range of attack (Radius of Gizmo around the player will give you the idea)
    public LayerMask enemyLayer;        // Layer for enemies (Layer name is Enemy, select the enemy cube and check the top right corner of the inspector)
    public float attackCoolDown = 0.3f; // Cooldown time between attacks
    public bool isAttacking = false;    // Flag to check if the player is currently attacking

    void Update()
    {
        //Check for attack input (left mouse button) and if the player is not already attacking
        if (!isAttacking && Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(Attacking());
        }
    }
    
    IEnumerator Attacking()
    {
        isAttacking = true;

        // Detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            transform.position, attackRange, enemyLayer);

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            // SAFETY FIX: Check if the EnemyHealth component actually exists before using it!
            EnemyHealth targetHealth = enemy.GetComponent<EnemyHealth>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(attackDamage);
            }
            else
            {
                Debug.LogWarning("Player hit something on the Enemy layer that doesn't have an EnemyHealth script: " + enemy.name);
            }
        }
        
        yield return new WaitForSeconds(attackCoolDown);
        isAttacking = false;
    }
    
    // Visualize attack range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}