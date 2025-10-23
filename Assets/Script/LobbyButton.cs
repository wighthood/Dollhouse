using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyButton : NetworkBehaviour
{
    public async void Disconnect()
    { 
        NetworkManager.Singleton.Shutdown(); 
        SceneManager.LoadScene("MainMenu");
    }
    
    
}
