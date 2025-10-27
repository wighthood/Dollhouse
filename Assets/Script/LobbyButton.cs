using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyButton : NetworkBehaviour
{
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
    
    [ClientRpc]
    private void DisconnectClientRpc()
    {
        TrueDisconnect();
    }
}
