using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PeriphericalCreep : NetworkBehaviour
{
    List<Camera> playerCams = new List<Camera>();
    PeriphericalDisappear periphericalDisappear;

    private void Start()
    {
        periphericalDisappear = GetComponent<PeriphericalDisappear>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && IsOwner)
        {
            TriggerEnterServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [Rpc(SendTo.Server)]
    private void TriggerEnterServerRpc(ulong clientId)
    {
        playerCams.Add(NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponentInChildren<Camera>());
        IsOnlyOnePlayerLooking();
    }

    private void OnTriggerExit(Collider other)
    {
        playerCams.Remove(other.GetComponentInChildren<Camera>());
        IsOnlyOnePlayerLooking();
    }

    void IsOnlyOnePlayerLooking()
    {
        if (playerCams.Count == 1)
        {
            SetPeriphericalDisappearClientRpc(true);
            return;
        }
        periphericalDisappear.ResetState();
        periphericalDisappear.enabled = false;
        SetPeriphericalDisappearClientRpc(false);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    void SetPeriphericalDisappearClientRpc(bool value)
    {
        periphericalDisappear.enabled = value;
        if (value)
        {
            periphericalDisappear.GetPlayerInfo(playerCams[0].gameObject);
        }
    }
    
}
