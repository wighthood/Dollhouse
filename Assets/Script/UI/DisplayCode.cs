using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class DisplayCode : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI codeDisplay;
    [SerializeField] private int maxPlayers = 4;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (IsHost)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += LimitPLayerAmount;
            codeDisplay.text = StaticCode.GameCode;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    private void LimitPLayerAmount(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count >= maxPlayers)
        {
            response.Approved = false;
            response.Reason = "Game is full";
        }
        else
        {
            response.Approved = true;
        }
    }
}
