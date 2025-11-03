using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PeriphericalCreep : NetworkBehaviour
{
    [SerializeField] PeriphericalDisappear periphericalDisappear;
    List<Camera> playerCams = new List<Camera>();
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCams.Add(other.GetComponentInChildren<Camera>(true));
            TriggerEnterServerRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void TriggerEnterServerRpc()
    {
        IsOnlyOnePlayerLooking();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCams.Remove(other.GetComponentInChildren<Camera>(true));
            TriggerExitServerRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void TriggerExitServerRpc()
    {
        IsOnlyOnePlayerLooking();
    }

    void IsOnlyOnePlayerLooking()
    {
        if (playerCams.Count == 1)
        {
            SetPeriphericalDisappearClientRpc();
            return;
        }
        periphericalDisappear.ResetStateClientRpc();
        periphericalDisappear.enabled = false;
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    void SetPeriphericalDisappearClientRpc()
    {
        periphericalDisappear.enabled = true;
        periphericalDisappear.GetPlayerInfo(playerCams[0].gameObject);
    }
}
