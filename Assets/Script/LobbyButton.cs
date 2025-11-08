using Unity.Netcode;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyButton : NetworkBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
    }

    public void Disconnect()
    {
        if (IsHost)
        {
            DisconnectServerRpc();
        }
        else
        {
            TrueDisconnect();
        }
    }

    private void TrueDisconnect()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    
    [Rpc(SendTo.Server)]
    private void DisconnectServerRpc()
    {
        DisconnectClientRpc();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void DisconnectClientRpc()
    {
        TrueDisconnect();
    }
}
