using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerRoleUI : NetworkBehaviour
{
    [SerializeField] private GameObject playerUIPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        
        GameObject uiInstance = Instantiate(playerUIPrefab);
        uiInstance.transform.SetParent(GameObject.Find("Canvas")?.transform, false);

        var roleText = uiInstance.transform.Find("RoleLabel")?.GetComponent<TextMeshProUGUI>();
        if (roleText != null)
        {
            roleText.text = IsHost ? "HOST" : "CLIENT";
        }
    }
}