using Unity.Netcode;
using UnityEngine;

public class DisableConnection : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!NetworkManager.Singleton.IsHost) return;
        NetworkManager.Singleton.ConnectionApprovalCallback = DisallowConnection;
    }

    private void DisallowConnection(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = false;
        response.Reason = "Game is in progress";
    }
}
