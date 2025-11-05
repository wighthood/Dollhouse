using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Vivox;
using Unity.VisualScripting;
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

        for (int i = 0; i < PlayersReady.Count; i++)
        {
            PlayersReady[i] = false;
            if (IsHost) IsReadyClientRpc(false,PlayersID[i+1],PlayersReady[i]);
        }
    }

    private void OnClientConnected(ulong obj)
    {
        if (obj == 0) return;
        PlayersID.Add(obj);
        PlayersReady.Add(false);
        if (IsServer) UpdatePlayerDisplaysClientRpc();
        SetReady(obj);
    }
    
    private void OnClientDisconnected(ulong obj)
    {
        if (obj == 0 || SceneManager.GetActiveScene().name != "Lobby") return;
        PlayersID.Remove(obj);
        PlayersReady.RemoveAt((int)obj-1);
        SetReady(obj);
        if (IsServer) UpdatePlayerDisplaysClientRpc();
    }

    #region ready
    private void IsReady()
    {
        IsReadyServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.Server)]
    private void IsReadyServerRpc(ulong clientId)
    {
        if (clientId == 0) return;
        PlayersReady[(int)clientId-1] = !PlayersReady[(int)clientId-1];
        SetReady(clientId);    
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
    
    private void SetReady(ulong clientId)
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
        int i = 0;
        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            GameObject newPlayer=Instantiate(playerPrefab, transform.GetChild(i).position, Quaternion.identity);
            newPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(player.ClientId);
            newPlayer.GetComponent<Controls>().SetPlayerPrefabRPC();
            //VivoxService.Instance.Set3DPosition(player.PlayerObject.gameObject, StaticCode.GameCode);
            i++;
        }
        
        LoadGame();
    }
    
    void LoadGame()
    {
        if (VivoxService.Instance.ActiveChannels?.Count > 0)
        {
            var channParts = VivoxService.Instance.ActiveChannels[StaticCode.GameCode];
            Debug.Log(StaticCode.GameCode + " Participants COUNT: " + channParts?.Count);
        }
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
