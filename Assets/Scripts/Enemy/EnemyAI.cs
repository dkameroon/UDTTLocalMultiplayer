using Unity.Netcode;
using UnityEngine;

public class EnemyAI : NetworkBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackInterval = 2.5f;
    [SerializeField] private float attackRange = 15f;
    [SerializeField] private LayerMask lineOfSightMask;

    private ITargetProvider targetProvider;
    private float timer;

    private void Awake()
    {
        targetProvider = new TargetFinder();
    }

    private void Update()
    {
        if (!IsServer) return;

        NetworkObject target = targetProvider.GetTarget(transform.position);
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > attackRange) return;

        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.forward = direction;

        // Line of sight check
        Ray ray = new Ray(firePoint.position, direction);
        if (Physics.Raycast(ray, out RaycastHit hit, attackRange, lineOfSightMask))
        {
            if (!hit.collider.CompareTag("Player")) return;
        }

        timer += Time.deltaTime;
        if (timer >= attackInterval)
        {
            timer = 0;
            FireProjectile(direction);
        }
    }

    private void FireProjectile(Vector3 direction)
    {
        if (!IsServer) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
        proj.GetComponent<NetworkObject>().Spawn();

        if (proj.TryGetComponent<IProjectile>(out var projectile))
        {
            projectile.Initialize(direction);
        }
    }
}