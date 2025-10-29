using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Controls : NetworkBehaviour
{
    Camera cam;
    private PlayerInput playerInput;
    private AudioListener audioListener;
    [SerializeField] private GameObject Menu;
    
    [Header("movement")]
    [SerializeField] float movementSpeed=1;
    [SerializeField] float rotationSpeed=1;
    Movements movements;

    [Header("screenshot")]
    [SerializeField] private GameObject photoPrefab;
    
    private void Awake()
    {
        cam = transform.GetComponentInChildren<Camera>();
        playerInput = GetComponent<PlayerInput>(); 
        movements = GetComponent<Movements>();
        movements.cam = cam;
        audioListener = cam.GetComponent<AudioListener>();
    }


    [Rpc(SendTo.ClientsAndHost)]
    public void SetPlayerPrefabRPC()
    {
        
        if (IsOwner)
        {
            Debug.Log(NetworkManager.Singleton.LocalClientId);
            playerInput.enabled = true;
            cam.enabled = true;
            audioListener.enabled = true;
        }
    }

    public void OpenMenu(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        Menu.SetActive(!Menu.activeSelf);
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
            movements.movement = new Vector3(input.x, 0, input.y) * movementSpeed * Time.deltaTime;
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

    IEnumerator Screenshot()
    {
        yield return new WaitForEndOfFrame();
        int width = Screen.width;
        int height= Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
            
        GameObject newphoto = Instantiate(photoPrefab,transform.position,transform.rotation);
        newphoto.GetComponent<Renderer>().material.mainTexture = tex;
    }
    
    
    
    public void TakeScreenshot(InputAction.CallbackContext context)
    {
        if(IsOwner && !Menu.activeSelf)
        {
            StartCoroutine(Screenshot());
        }
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
        Vector3 newRotation=actualRotation+(new Vector3(-input.y,input.x,0)*rotationSpeed*Time.deltaTime);
        cam.transform.localRotation=Quaternion.Euler(newRotation);
    }

}