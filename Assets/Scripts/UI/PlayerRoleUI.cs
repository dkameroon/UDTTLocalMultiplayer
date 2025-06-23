using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerRoleUI : NetworkBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject playerUIPrefab;

    private GameObject uiInstance;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        
        Transform canvasTransform = GameObject.Find("Canvas")?.transform;
        if (canvasTransform == null)
        {
            Debug.LogWarning("Canvas not found in the scene. PlayerRoleUI will not be displayed.");
            return;
        }

        // Instantiate the UI prefab as a child of the Canvas
        uiInstance = Instantiate(playerUIPrefab, canvasTransform, false);

        // Find the RoleLabel Text component in the instantiated UI
        var roleText = uiInstance.transform.Find("RoleLabel")?.GetComponent<TextMeshProUGUI>();
        if (roleText == null)
        {
            Debug.LogWarning("RoleLabel TextMeshProUGUI not found in playerUIPrefab.");
            return;
        }

        // Set the text based on whether this client is Host or Client
        roleText.text = IsHost ? "HOST" : "CLIENT";
    }

    private void OnDestroy()
    {
        if (uiInstance != null)
        {
            Destroy(uiInstance);
        }
    }
}