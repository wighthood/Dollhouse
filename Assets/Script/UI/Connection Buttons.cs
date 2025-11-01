using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectionButton : MonoBehaviour
    {
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _joinButton;
        
        [SerializeField] private TMP_InputField inputField;
        private void Start()
        {
            _hostButton.onClick.AddListener(OnHostButtonClicked);
            _joinButton.onClick.AddListener(OnJoinButtonClicked);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public async void OnJoinButtonClicked()
        { 
            await StartClientWithRelay(inputField.text, "udp");
            
            StaticCode.GameCode= inputField.text;
            
            SwitchToGameScene();
        }

        public async void OnHostButtonClicked()
        {
            string joinCode = await StartHostWithRelay(15, "udp");
            inputField.gameObject.SetActive(false);
            StaticCode.GameCode = joinCode;
            Debug.Log(joinCode);
            SwitchToGameScene();
        }

        private async Task<string> StartHostWithRelay(int maxConnections, string connectionType)
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync(); //Todo: handle sign in elsewhere && not anonymously
            }
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData(connectionType));
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return NetworkManager.Singleton.StartHost() ? joinCode : null;
        }

        private async Task<bool> StartClientWithRelay(string joinCode, string connectionType)
        {
            joinCode=joinCode.ToUpper();
            if (string.IsNullOrEmpty(joinCode))
            {
                Debug.LogError("Join code is null or empty");
                return false;
            }
            
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync(); //Todo: handle sign in elsewhere && not anonymously
            }

            try
            {
                var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData(connectionType));
            }
            catch (RelayServiceException e)
            {
                Debug.LogError($"Relay join failed: {e.Message}");
                return false;
            }
            return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
        }

        private void SwitchToGameScene()
        {
            if (!NetworkManager.Singleton.IsHost) return;
            NetworkManager.Singleton.SceneManager.LoadScene(
                "Lobby",
                LoadSceneMode.Single); 
        }

        public void Quit()
        {
            Application.Quit();
        }
    }