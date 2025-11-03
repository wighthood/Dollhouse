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
        playerCams.Add(other.GetComponentInChildren<Camera>());
        SetPeriphericalDisappear(IsOnlyOnePlayerLooking(playerCams));
    }

    private void OnTriggerExit(Collider other)
    {
        playerCams.Remove(other.GetComponentInChildren<Camera>());
        SetPeriphericalDisappear(IsOnlyOnePlayerLooking(playerCams));
    }

    bool IsOnlyOnePlayerLooking(List<Camera> playerCam)
    {
        if (playerCam.Count == 1)
        {
            return true;
        }
        periphericalDisappear.ResetState();
        periphericalDisappear.enabled = false;
        return false;
    }
    
    void SetPeriphericalDisappear(bool value)
    {
        periphericalDisappear.enabled = value;
        if (value)
        {
            periphericalDisappear.GetPlayerInfo(playerCams[0].gameObject);
        }
    }
    
}
