using Unity.Netcode;
using UnityEngine;

public class TargetFinder : ITargetProvider
{
    private const float MaxRayDistance = 100f;
    private const float EyeHeightOffset = 1f;

    public NetworkObject GetTarget(Vector3 fromPosition)
    {
        float closestDistance = float.MaxValue;
        NetworkObject closestPlayer = null;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var player = client.PlayerObject;
            if (!IsValidTarget(player)) continue;

            Vector3 origin = fromPosition + Vector3.up * EyeHeightOffset;
            Vector3 target = player.transform.position + Vector3.up * EyeHeightOffset;
            Vector3 direction = (target - origin).normalized;

            if (!HasLineOfSight(origin, direction, player)) continue;

            float distance = Vector3.Distance(fromPosition, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        return closestPlayer;
    }
    
    // Checks if a player is valid for targeting (exists, alive, and spawned).
    private bool IsValidTarget(NetworkObject player)
    {
        if (player == null || !player.IsSpawned)
            return false;

        var health = player.GetComponent<Health>();
        return health != null && health.IsAlive();
    }
    
    // Checks if there is an unobstructed ray between enemy and player.
    private bool HasLineOfSight(Vector3 origin, Vector3 direction, NetworkObject player)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, MaxRayDistance))
        {
            return hit.collider.gameObject == player.gameObject;
        }
        return false;
    }
}