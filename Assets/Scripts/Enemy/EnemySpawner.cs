using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private Vector3 spawnRange = new Vector3(50, 1, 30);
    [SerializeField] private int maxEnemies = 10;
    [SerializeField] private float minDistanceBetweenEnemies = 5f;
    [SerializeField] private float wallCheckRadius = 5f;
    [SerializeField] private LayerMask wallLayerMask;

    private float timer;
    private readonly List<NetworkObject> spawnedEnemies = new List<NetworkObject>();

    private void Update()
    {
        if (!IsServer) return;

        spawnedEnemies.RemoveAll(enemy => enemy == null || !enemy.IsSpawned);

        if (spawnedEnemies.Count >= maxEnemies) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0;

            for (int i = 0; i < maxEnemies; i++)
            {
                Vector3 spawnPos = new Vector3(
                    Random.Range(-spawnRange.x, spawnRange.x),
                    1,
                    Random.Range(-spawnRange.z, spawnRange.z)
                );

                if (!IsOverlappingOtherEnemies(spawnPos) && !IsNearWall(spawnPos))
                {
                    GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                    var netObj = enemy.GetComponent<NetworkObject>();
                    netObj.Spawn();
                    spawnedEnemies.Add(netObj);
                    break;
                }
            }
        }
    }

    private bool IsOverlappingOtherEnemies(Vector3 newPos)
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy == null || !enemy.IsSpawned) continue;
            float dist = Vector3.Distance(enemy.transform.position, newPos);
            if (dist < minDistanceBetweenEnemies)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsNearWall(Vector3 position)
    {
        return Physics.CheckSphere(position, wallCheckRadius, wallLayerMask);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRange.x * 2, 0.1f, spawnRange.z * 2));
    }
}
