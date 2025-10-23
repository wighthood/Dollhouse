using Unity.Netcode;
using UnityEngine.SceneManagement;

public class AutoLoadMainMenu : NetworkBehaviour
{
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if(IsHost) return;
        SceneManager.LoadScene("MainMenu");
    }

}
