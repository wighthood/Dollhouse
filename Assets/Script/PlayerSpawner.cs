using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : NetworkBehaviour
{
    [Header("start/ready button")]
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private Button button;
    
    [Header("player display")]
    [SerializeField] private GameObject playersDisplay; 
    [SerializeField] private GameObject playerDisplayPrefab;
        
    
    private List<ulong> PlayersID = new();
    private List<bool> PlayersReady = new();
    private Dictionary<ulong, GameObject> _displays = new();
    
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        if (IsHost)
        {
            button.onClick.AddListener(SpawnPlayer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Start Game";
            button.interactable = true;
            UpdatePlayerDisplaysClientRpc();
        }
        else
        {
            button.onClick.AddListener(IsReady);
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
        }
    }

    private void OnClientConnected(ulong obj)
    {
        if (obj == 0) return;
        PlayersID.Add(obj);
        PlayersReady.Add(false);
        if (IsServer) UpdatePlayerDisplaysClientRpc();
        CheckReady(obj);
    }
    
    private void OnClientDisconnected(ulong obj)
    {
        if (obj == 0) return;
        PlayersID.Remove(obj);
        PlayersReady.RemoveAt((int)obj);
        if (IsServer) UpdatePlayerDisplaysClientRpc();
        CheckReady(obj);
    }

    #region ready
    private void IsReady()
    {
        IsReadyServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.Server)]
    private void IsReadyServerRpc(ulong clientId)
    {
        PlayersReady[(int)clientId-1] = !PlayersReady[(int)clientId-1];
        CheckReady(clientId);    
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void IsReadyClientRpc(bool allReady, ulong clientId,bool ready)
    {
        _displays[clientId].GetComponentInChildren<Image>().color = ready ? Color.green : Color.red;
        if (IsHost)
        {
            button.interactable = allReady;
        }
    }
    
    private void CheckReady(ulong clientId = 0)
    {
        if (PlayersReady.Count > 0)
        {
            for (int i = 0; i < PlayersReady.Count; i++)
            {
                if (!PlayersReady[i])
                {
                    IsReadyClientRpc(false,PlayersID[i], PlayersReady[i]);
                    return;
                }
            }
        }
        IsReadyClientRpc(true,clientId,PlayersReady[(int)clientId-1]);
    }
    #endregion
    
    private void SpawnPlayer()
    {
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
    
    [Rpc(SendTo.ClientsAndHost)]
    private void UpdatePlayerDisplaysClientRpc()
    {
        foreach (var display in _displays)
        {
            Destroy(display.Value);
        }

        _displays.Clear();
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            _displays.Add(client.ClientId,Instantiate(playerDisplayPrefab, playersDisplay.transform));
            if (client.ClientId == 0)
            {
                _displays[0].GetComponentInChildren<Image>().color = Color.green;
            }
            _displays[client.ClientId].GetComponent<TextMeshProUGUI>().text = "player : " + client.ClientId;
        }
    }
}
