using Unity.Netcode;
using UnityEngine;

public class TargetFinder : ITargetProvider
{
    public NetworkObject GetTarget(Vector3 fromPosition)
    {
        float closestDistance = float.MaxValue;
        NetworkObject closestPlayer = null;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var player = client.PlayerObject;
            if (player == null) continue;

            var health = player.GetComponent<Health>();
            if (health == null || !health.IsAlive) continue;

            float distance = Vector3.Distance(player.transform.position, fromPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        return closestPlayer;
    }
}