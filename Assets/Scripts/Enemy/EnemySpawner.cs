using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : NetworkBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxEnemies = 10;

    [Header("Spawn Area")]
    [SerializeField] private Vector3 spawnRange = new Vector3(50f, 1f, 30f);

    [Header("Validation")]
    [SerializeField] private float minDistanceBetweenEnemies = 5f;
    [SerializeField] private float wallCheckRadius = 5f;
    [SerializeField] private LayerMask wallLayerMask;

    private readonly List<NetworkObject> spawnedEnemies = new List<NetworkObject>();
    private float spawnTimer;

    private void Update()
    {
        if (!IsServer) return;

        CleanUpDeadEnemies();

        if (spawnedEnemies.Count >= maxEnemies) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnEnemy();
        }
    }

    // Removes any dead or despawned enemies from the list.
    private void CleanUpDeadEnemies()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null || !enemy.IsSpawned);
    }
    
    // Attempts to find a valid spawn position and spawn one enemy.
    private void TrySpawnEnemy()
    {
        const int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();

            if (IsValidSpawnPosition(spawnPosition))
            {
                SpawnEnemy(spawnPosition);
                break;
            }
        }
    }
    
    // Returns a random position within the spawn range.
    private Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(
            Random.Range(-spawnRange.x, spawnRange.x),
            1f,
            Random.Range(-spawnRange.z, spawnRange.z)
        );
    }
    
    // Validates that the spawn position is not too close to walls or other enemies.
    private bool IsValidSpawnPosition(Vector3 position)
    {
        return !IsNearWall(position) && !IsTooCloseToOtherEnemies(position);
    }

    private bool IsNearWall(Vector3 position)
    {
        return Physics.CheckSphere(position, wallCheckRadius, wallLayerMask);
    }

    private bool IsTooCloseToOtherEnemies(Vector3 position)
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy == null || !enemy.IsSpawned) continue;

            float distance = Vector3.Distance(enemy.transform.position, position);
            if (distance < minDistanceBetweenEnemies) return true;
        }
        return false;
    }
    
    // Instantiates and spawns an enemy at the given position.
    private void SpawnEnemy(Vector3 position)
    {
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        NetworkObject netObj = enemy.GetComponent<NetworkObject>();

        if (netObj != null)
        {
            netObj.Spawn();
            spawnedEnemies.Add(netObj);
        }
        else
        {
            Debug.LogError("Enemy prefab is missing NetworkObject component.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 size = new Vector3(spawnRange.x * 2, 0.1f, spawnRange.z * 2);
        Gizmos.DrawWireCube(transform.position, size);
    }
}
