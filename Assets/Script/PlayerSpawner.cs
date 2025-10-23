using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : NetworkBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;
    public void SpawnPlayer()
    {
        if (!IsServer)
        {
            return;
        }
        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            GameObject newPlayer=Instantiate(playerPrefab, transform.position, Quaternion.identity);
            newPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(player.ClientId);
            newPlayer.GetComponent<Controls>().SetPlayerPrefabRPC();
        }
        
        LoadGame();
    }

    void LoadGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
