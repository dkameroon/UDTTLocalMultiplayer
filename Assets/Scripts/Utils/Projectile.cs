using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(Collider))]
public class Projectile : NetworkBehaviour, IProjectile
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private int damage = 10;

    private Vector3 direction;
    private float timer;

    public void Initialize(Vector3 direction)
    {
        if (!IsServer) return;
        this.direction = direction.normalized;
    }

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

        transform.position += direction * speed * Time.deltaTime;
        timer += Time.deltaTime;

        if (timer >= lifetime)
        {
            Despawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.TryGetComponent<IDamageable>(out var target))
        {
            target.TakeDamage(damage);
        }

        Despawn();
    }

    private void Despawn()
    {
        if (!IsServer) return;

        if (NetworkObject != null && NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn();
        }
    }
}