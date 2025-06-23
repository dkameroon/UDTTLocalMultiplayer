using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour, IDamageable
{
    [Header("Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private PlayerHealthBar healthBarPrefab;

    private readonly NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    private PlayerHealthBar healthBarInstance;

    #region Unity Lifecycle

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        if (IsOwner)
        {
            SpawnHealthBar();
        }

        currentHealth.OnValueChanged += HandleHealthChanged;
    }

    private void OnDestroy()
    {
        currentHealth.OnValueChanged -= HandleHealthChanged;

        if (IsOwner && healthBarInstance != null)
        {
            Destroy(healthBarInstance.gameObject);
        }
    }


    public void TakeDamage(int amount)
    {
        if (!IsServer || !IsAlive()) return;

        currentHealth.Value = Mathf.Max(currentHealth.Value - amount, 0);
    }

    public bool IsAlive()
    {
        return currentHealth.Value > 0;
    }

    private void HandleHealthChanged(int oldValue, int newValue)
    {
        UpdateHealthBar();

        if (IsServer && newValue <= 0 && oldValue > 0)
        {
            Debug.Log($"{OwnerClientId} died.");

            HidePlayerClientRpc();
            NotifyClientOfDeathClientRpc(OwnerClientId);
        }
    }

    private void SpawnHealthBar()
    {
        Transform container = GameObject.Find("PlayersHealthContainer")?.transform;
        if (container != null && healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, container);
            UpdateHealthBar();
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
    

    [ServerRpc(RequireOwnership = false)]
    public void RespawnServerRpc(Vector3 spawnPosition)
    {
        if (!IsServer) return;

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
    private void NotifyClientOfDeathClientRpc(ulong deadClientId)
    {
        if (deadClientId == NetworkManager.Singleton.LocalClientId)
        {
            UIManager.Instance?.ShowRespawnButton();
        }
    }

    #endregion
}
