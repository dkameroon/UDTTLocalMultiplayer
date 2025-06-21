using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MenuUI : MonoBehaviour
{
    public static MenuUI Instance { get; private set; }

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

        Vector3 spawnPosition = new Vector3(
            Random.Range(-10f, 10f),
            1f,
            Random.Range(-10f, 10f)
        );

        health.RespawnServerRpc(spawnPosition);
        HideRespawnButton();
    }
}
