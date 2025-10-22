using Unity.Netcode;
using UnityEngine;

public class ConnectionButtons : MonoBehaviour
{
    private NetworkManager _networkManager;
    private void Start()
    {
        _networkManager = NetworkManager.Singleton;
    }

    public void Connect()
    {
        _networkManager.StartClient();
    }

    public void StartServer()
    {
        _networkManager.StartServer();
    }

    public void StartHost()
    {
        _networkManager.StartHost();
    }
}
