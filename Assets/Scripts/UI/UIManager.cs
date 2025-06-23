using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [Header("UI References")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private Button respawnButton;
    

    private bool isMenuVisible = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        HideRespawnButton();

        if (respawnButton != null)
        {
            respawnButton.onClick.AddListener(OnRespawnButtonClicked);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
        {
            HideMenu();
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            HideMenu();
        }
    }

    private void Update()
    {
        // Only allow menu toggle if network session active
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
        
    }

    private void ToggleMenu()
    {
        isMenuVisible = !isMenuVisible;
        menuUI.SetActive(isMenuVisible);
    }

    private void HideMenu()
    {
        isMenuVisible = false;
        if (menuUI != null)
            menuUI.SetActive(false);
    }

    public void ShowRespawnButton()
    {
        if (respawnButton != null)
            respawnButton.gameObject.SetActive(true);
    }

    public void HideRespawnButton()
    {
        if (respawnButton != null)
            respawnButton.gameObject.SetActive(false);
    }


    private void OnRespawnButtonClicked()
    {
        var localPlayer = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        if (localPlayer == null)
        {
            return;
        }

        var health = localPlayer.GetComponent<Health>();
        if (health == null)
        {
            return;
        }
        // Generate a spawn position - consider making this deterministic or from a spawn manager in a real game
        Vector3 spawnPosition = new Vector3(
                Random.Range(10f, -5f),
                1f,
                Random.Range(-35f, -45f)
        );

        health.RespawnServerRpc(spawnPosition);
        HideRespawnButton();
    }
}