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

namespace UI
{
    public class ConnectionButton : MonoBehaviour
    {
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _joinButton;
        
        [SerializeField] private TMP_InputField inputField;

        static string GameCode;
        private void Start()
        {
            _hostButton.onClick.AddListener(OnHostButtonClicked);
            _joinButton.onClick.AddListener(OnJoinButtonClicked);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public async void OnJoinButtonClicked()
        {
            _hostButton.interactable = false;
            _joinButton.interactable = false;
            bool _isConnected = await StartClientWithRelay(inputField.text, "udp");

            if (!_isConnected)
            {
                _hostButton.interactable = true;
                _joinButton.interactable = true;
                return;
            }

            GameCode= inputField.text;
            
            SwitchToGameScene();
        }

        public async void OnHostButtonClicked()
        {
            _hostButton.interactable = false;
            _joinButton.interactable = false;
            string _joinCode = await StartHostWithRelay(15, "udp");
            
            if (string.IsNullOrEmpty(_joinCode))
            {
                _hostButton.interactable = true;
                _joinButton.interactable = true;
                return;
            }
            
            inputField.gameObject.SetActive(false);
            Debug.Log("Join code: " + _joinCode);
            GameCode = _joinCode;

            SwitchToGameScene();
        }

        private async Task<string> StartHostWithRelay(int _maxConnections, string _connectionType)
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync(); //Todo: handle sign in elsewhere && not anonymously
            }
            var _allocation = await RelayService.Instance.CreateAllocationAsync(_maxConnections);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(_allocation.ToRelayServerData(_connectionType));
            var _joinCode = await RelayService.Instance.GetJoinCodeAsync(_allocation.AllocationId);
            return NetworkManager.Singleton.StartHost() ? _joinCode : null;
        }

        private async Task<bool> StartClientWithRelay(string _joinCode, string _connectionType)
        {
            if (string.IsNullOrEmpty(_joinCode))
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
                var _allocation = await RelayService.Instance.JoinAllocationAsync(joinCode: _joinCode);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(_allocation.ToRelayServerData(_connectionType));
            }
            catch (RelayServiceException e)
            {
                Debug.LogError($"Relay join failed: {e.Message}");
                return false;
            }
            return !string.IsNullOrEmpty(_joinCode) && NetworkManager.Singleton.StartClient();
        }

        private void SwitchToGameScene()
        {
            NetworkManager.Singleton.SceneManager.LoadScene(
                "GameScene",
                LoadSceneMode.Single);
        }
    }
}