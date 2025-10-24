using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyButton : NetworkBehaviour
{
    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }
}
