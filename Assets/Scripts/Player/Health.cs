using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private PlayerHealthBar healthBarPrefab;

    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();
    private PlayerHealthBar healthBarInstance;
    
    public bool IsAlive => currentHealth.Value > 0;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        if (IsOwner)
        {
            Transform container = GameObject.Find("PlayersHealthContainer")?.transform;
            if (container != null && healthBarPrefab != null)
            {
                healthBarInstance = Instantiate(healthBarPrefab, container);
                UpdateHealthBar();
            }
        }

        currentHealth.OnValueChanged += OnHealthChanged;
    }

    private void OnDestroy()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;

        if (IsOwner && healthBarInstance != null)
        {
            Destroy(healthBarInstance.gameObject);
        }
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        UpdateHealthBar();

        if (IsServer && newHealth <= 0 && oldHealth > 0)
        {
            Debug.Log($"{OwnerClientId} died.");
            HidePlayerClientRpc();
            NotifyClientOfDeathClientRpc(OwnerClientId);
        }
    }

    private void UpdateHealthBar()
    {
        if (IsOwner && healthBarInstance != null)
        {
            float normalized = Mathf.Clamp01((float)currentHealth.Value / maxHealth);
            healthBarInstance.SetHealth(normalized, currentHealth.Value, maxHealth);
        }
    }

    public void TakeDamage(int amount)
    {
        if (!IsServer || currentHealth.Value <= 0) return;

        currentHealth.Value = Mathf.Max(currentHealth.Value - amount, 0);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RespawnServerRpc(Vector3 spawnPosition)
    {
        currentHealth.Value = maxHealth;
        transform.position = spawnPosition;

        RespawnClientRpc();
    }

    [ClientRpc]
    private void RespawnClientRpc()
    {
        gameObject.SetActive(true);
        UpdateHealthBar();
    }

    [ClientRpc]
    private void HidePlayerClientRpc()
    {
        gameObject.SetActive(false);
    }

    [ClientRpc]
    private void NotifyClientOfDeathClientRpc(ulong diedClientId)
    {
        if (diedClientId == NetworkManager.Singleton.LocalClientId)
        {
            MenuUI.Instance?.ShowRespawnButton();
        }
    }
}
