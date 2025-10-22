using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerDisplay : NetworkBehaviour
{
    [SerializeField] private GameObject playerDisplay;
    [SerializeField] private GameObject playerList;
    private List<GameObject> _players = new();
    
    private void OnConnectedToServer()
    {
        if (IsServer)
        {
            GameObject newPlayer = Instantiate(playerDisplay, playerList.transform);
            newPlayer.GetComponent<TextMeshProUGUI>().text = "player " + NetworkManager.Singleton.LocalClientId;
            _players.Add(newPlayer);
        }
    }
}
