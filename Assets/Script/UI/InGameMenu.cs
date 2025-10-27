using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenu : NetworkBehaviour
{
    [SerializeField] private Button button;

    void Start()
    {
        if (IsHost)
        {
            button.onClick.AddListener(ReturnToLobby);
        }
        else
        {
            button.onClick.AddListener(Disconnect);
        }
    }

    private void ReturnToLobby()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(
            "Lobby",
            LoadSceneMode.Single);
        foreach (var networkObject in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            networkObject.Despawn(true);
        }
    }
    
    private void Disconnect()
    {
        if (!IsOwner) return;
        SceneManager.LoadScene("MainMenu");
        NetworkManager.Singleton.Shutdown();
    }
}
