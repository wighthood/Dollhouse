using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerDisplay : NetworkBehaviour
{
    [SerializeField] private GameObject playersDisplay;
    [SerializeField] private GameObject playerDisplayPrefab;
    
    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            UpdatePlayerDisplaysClientRpc();
        }
        
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
            UpdatePlayerDisplaysClientRpc();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (IsServer)
            UpdatePlayerDisplaysClientRpc();
    }

    [ClientRpc]
    private void UpdatePlayerDisplaysClientRpc()
    {
        foreach (Transform child in playersDisplay.transform)
            Destroy(child.gameObject);

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var display = Instantiate(playerDisplayPrefab, playersDisplay.transform);
            display.GetComponent<TextMeshProUGUI>().text = "player : " + client.ClientId;
        }
    }
}
