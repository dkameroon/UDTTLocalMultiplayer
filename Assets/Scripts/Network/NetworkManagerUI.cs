using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button shutdownButton;
    [SerializeField] private GameObject background;

    private void Awake()
    {
        background.SetActive(true);
        hostButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            NetworkManager.Singleton.StartHost();
            background.SetActive(false);
        });

        clientButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            NetworkManager.Singleton.StartClient();
            background.SetActive(false);
        });

        shutdownButton.onClick.AddListener(() =>
        {
            Time.timeScale = 0f;
            NetworkManager.Singleton.Shutdown();
            background.SetActive(true);
        });
    }
}