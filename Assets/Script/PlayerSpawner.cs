using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : NetworkBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private Button button;

    private Dictionary<ulong, bool> _players = new();
    
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        if (IsHost)
        {
            button.onClick.AddListener(SpawnPlayer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Start Game";
            button.interactable = false;
        }
        else
        {
            button.onClick.AddListener(IsReadyServer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
        }
    }

    private void OnClientConnected(ulong obj)
    {
        if (obj == 0) return;
        _players.Add(obj, false);
    }
    
    private void OnClientDisconnected(ulong obj)
    {
        if (obj == 0) return;
        _players.Remove(obj);
    }

    private void IsReadyServer()
    {
        _players[NetworkManager.Singleton.LocalClientId] = true;
        if (IsHost)
        {
            foreach (var player in _players)
                if (!player.Value)
                {
                    button.interactable = false;
                    return;
                }

            button.interactable = true;
        }
    }

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
}
