using UnityEngine;

public class SpecificEnemySpawner : MonoBehaviour
{
    [Header("Spawner Configuration")]

    // Spawn prefab.
    public GameObject enemyPrefabToSpawn;

    // Number of enemy that spawn from each spawner.
    public int totalEnemiesToSpawn = 3;

    // The area size where enemies will scatter. Increase this if they spawn inside each other.
    public float spawnRadius = 1.5f;

    void Start()
    {
        for (int currentEnemyIndex = 0; currentEnemyIndex < totalEnemiesToSpawn; currentEnemyIndex++)
        {
            SpawnSingleEnemy();
        }
    }

    void SpawnSingleEnemy()
    {
        // Gets a random position inside a circle to scatter the spawns.
        Vector2 randomSpawnOffset = Random.insideUnitCircle * spawnRadius;
        Vector2 finalSpawnPosition = (Vector2)transform.position + randomSpawnOffset;

        Instantiate(enemyPrefabToSpawn, finalSpawnPosition, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        // Draws a red circle in the editor to show the scatter area.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}