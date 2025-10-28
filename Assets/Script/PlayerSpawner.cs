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
        
    
    private Dictionary<ulong, bool> _players = new();
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
        _players.Add(obj, false);
        if (IsServer) UpdatePlayerDisplaysClientRpc();
        CheckReadyClientRpc(obj);
    }
    
    private void OnClientDisconnected(ulong obj)
    {
        if (obj == 0) return;
        _players.Remove(obj);
        if (IsServer) UpdatePlayerDisplaysClientRpc();
        CheckReadyClientRpc(obj);
    }

    #region ready
    private void IsReady()
    {
        IsReadyServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.Server)]
    private void IsReadyServerRpc(ulong clientId)
    {
        _players[clientId] = !_players[clientId];
        CheckReadyClientRpc(clientId);    
    }
    
    private void Is_Ready(bool allReady = true, ulong clientId = 0)
    {
        foreach (var VARIABLE in _displays)
        {
            Debug.Log(VARIABLE.Key);
        }
        _displays[clientId].GetComponentInChildren<Image>().color = _players[clientId] ? Color.green : Color.red;
        if (IsHost)
        {
            button.interactable = allReady;
        }
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void CheckReadyClientRpc(ulong clientId = 0)
    {
        if (_players.Count > 0)
        {
            foreach (var player in _players)
                if (!player.Value)
                {
                    Is_Ready(false,player.Key);
                    return;
                }
        }
        Is_Ready(true,clientId);
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
