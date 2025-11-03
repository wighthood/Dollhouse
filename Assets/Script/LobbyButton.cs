using Unity.Netcode;
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
        SceneManager.LoadScene("MainMenu");
        NetworkManager.Singleton.Shutdown();
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
