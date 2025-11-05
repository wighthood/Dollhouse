using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class AutoLoadMainMenu : NetworkBehaviour
{
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += DisconnectClient;
    }

    private void DisconnectClient(ulong clientId)
    {
        if(IsHost || !IsOwner && clientId != 0) return;
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }
}
