using System;
using Unity.Netcode;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Controls : NetworkBehaviour
{
    Camera cam;
    private PlayerInput playerInput;
    private AudioListener audioListener;
    [SerializeField] private GameObject Menu;
    
    [SerializeField] float movementSpeed=1;
    [SerializeField] float rotationSpeed=1;
    [SerializeField] private GameObject audioSettings;
    private ChatAudioUpdate playerChat;
    [SerializeField] private GameObject Inventory;
    [SerializeField] private Animator animator;
    Movements movements;

    
    private void Awake()
    {
        cam = transform.GetComponentInChildren<Camera>();
        playerInput = GetComponent<PlayerInput>(); 
        movements = GetComponent<Movements>();
        movements.cam = cam;
        audioListener = cam.GetComponent<AudioListener>();
        playerChat = GetComponent<ChatAudioUpdate>();
        playerChat.cam = cam;
        movements.animator=animator;
        
    }

    void ActivateAudioUpdate(string channel)
    {
        if (IsOwner)
        {
            if (playerChat != null)
            {
                playerChat.enabled = true;
            }
        }
        
    }

    void DeactivateAudioUpdate(string channel)
    {
        if (IsOwner)
        {
            if (playerChat != null)
            {
                playerChat.enabled = false;
            }
            
        }
    }
    
    
    [Rpc(SendTo.ClientsAndHost)]
    public void SetPlayerPrefabRPC()
    {
        
        if (IsOwner)
        {
            playerInput.enabled = true;
            cam.enabled = true;
            audioListener.enabled = true;
            Inventory.SetActive(true);
            VivoxService.Instance.ChannelJoined += ActivateAudioUpdate;
            VivoxService.Instance.ChannelLeft += DeactivateAudioUpdate;
            VivoxService.Instance.JoinPositionalChannelAsync(StaticCode.GameCode, ChatCapability.AudioOnly,new Channel3DProperties(16,1,1.0f,AudioFadeModel.InverseByDistance));
            
        }
    }

    public void OpenMenu(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        Cursor.visible = !Menu.activeSelf;
        Menu.SetActive(!Menu.activeSelf);
        audioSettings.SetActive(!audioSettings.activeSelf);
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!IsOwner || Menu.activeSelf)
        {
            return;
        }
        if (context.started)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            MoveToServerRPC(true,moveInput);
        }

        if (context.performed)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            MoveToServerRPC(true,moveInput);
        }

        if (context.canceled)
        {
            MoveToServerRPC(false);
        }
        
    }
    
    [Rpc(SendTo.Server)]
    void MoveToServerRPC(bool moving,Vector2 input=new Vector2())
    {
        MoveToClientRPC(moving, input);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void MoveToClientRPC(bool moving, Vector2 input = new Vector2())
    {
        if (moving)
        {
            movements.enabled = true;
            movements.movement = new Vector3(input.x, 0, input.y) * movementSpeed;
            
            return;
        }
        movements.enabled = false;
    }
    
    public void Rotate(InputAction.CallbackContext context)
    {
        if (!IsOwner || Menu.activeSelf)
        {
            return;
        }
        Vector2 mouseMovement=context.ReadValue<Vector2>();
        RotateToServerRPC(mouseMovement);
    }

    [Rpc(SendTo.Server)]
    void RotateToServerRPC(Vector2 input)
    {
        RotateToClientRPC(input);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void RotateToClientRPC(Vector2 input)
    {
        Vector3 actualRotation=cam.transform.localRotation.eulerAngles;
        Vector3 newRotation=actualRotation+new Vector3(-input.y,0,0)*(rotationSpeed * NetworkManager.ServerTime.FixedDeltaTime);
        if (newRotation.x>180f) newRotation.x-=360f;
        newRotation.x=Mathf.Clamp(newRotation.x,-90f,90f);
        cam.transform.localRotation=Quaternion.Euler(newRotation);
        actualRotation=transform.rotation.eulerAngles;
        newRotation = actualRotation + new Vector3(0, input.x, 0) *(rotationSpeed *NetworkManager.ServerTime.FixedDeltaTime);
        transform.rotation = Quaternion.Euler(newRotation);
    }
    
    public void SetTchatVolume()
    {
        VivoxService.Instance.SetOutputDeviceVolume(Mathf.FloorToInt(audioSettings.GetComponentInChildren<Slider>().value));
    }
}