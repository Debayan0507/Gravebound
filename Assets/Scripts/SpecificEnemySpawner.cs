using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpecificEnemySpawner : MonoBehaviour
{
    [Header("Spawner Configuration")]
    public GameObject groundPrefab;
    public GameObject flyingPrefab;

    public int maxEnemies = 3;
    public float spawnRadius = 1.5f;
    public float respawnDelay = 5f;

    private List<GameObject> groundGhosts = new List<GameObject>();
    private List<GameObject> flyingGhosts = new List<GameObject>();
    private bool isSpawning = false;

    void Start()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnSingleEnemy();
        }
    }

    void Update()
    {
        // Purge dead enemies from lists
        groundGhosts.RemoveAll(ghost => ghost == null);
        flyingGhosts.RemoveAll(ghost => ghost == null);

        int totalActive = groundGhosts.Count + flyingGhosts.Count;

        if (totalActive < maxEnemies && !isSpawning)
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        isSpawning = true;
        yield return new WaitForSeconds(respawnDelay);

        int totalActive = groundGhosts.Count + flyingGhosts.Count;
        if (totalActive < maxEnemies)
        {
            SpawnSingleEnemy();
        }

        isSpawning = false;
    }

    void SpawnSingleEnemy()
    {
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        Vector2 spawnPos = (Vector2)transform.position + randomOffset;

        // Determine which type is lacking, default to random if equal
        bool spawnGround = groundGhosts.Count < flyingGhosts.Count;
        if (groundGhosts.Count == flyingGhosts.Count) spawnGround = Random.value > 0.5f;

        GameObject prefabToSpawn = spawnGround ? groundPrefab : flyingPrefab;
        GameObject newGhost = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        if (spawnGround) groundGhosts.Add(newGhost);
        else flyingGhosts.Add(newGhost);
    }
}