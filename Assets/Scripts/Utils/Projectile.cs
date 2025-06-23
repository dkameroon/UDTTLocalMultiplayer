using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(Collider))]
public class Projectile : NetworkBehaviour, IProjectile
{
    [Header("Projectile Settings")]
    [SerializeField]
    private float speed = 10f;

    [SerializeField]
    private float lifetime = 3f;

    [SerializeField]
    private int damage = 10;

    private Vector3 direction;
    private float timer;

    
    // Initializes the projectile with a normalized direction vector
    public void Initialize(Vector3 direction)
    {
        if (!IsServer) return;
        this.direction = direction.normalized;
    }

    
    // Called when the NetworkObject spawns. Disables the script on clients to ensure server authority
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        timer = 0f;
    }

    private void Update()
    {
        if (!IsServer) return;

        MoveProjectile();
        HandleLifetime();
    }

    
    // Moves the projectile forward based on direction and speed.
    private void MoveProjectile()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    
    // Checks if lifetime exceeded and despawns the projectile.
    private void HandleLifetime()
    {
        timer += Time.deltaTime;

        if (timer >= lifetime)
        {
            Despawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damage);
        }

        Despawn();
    }

    
    // Despawns the projectile network object safely.
    private void Despawn()
    {
        if (!IsServer) return;

        if (NetworkObject != null && NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn();
        }
    }
}
