using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

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

    IEnumerator Screenshot(int width, int height)
    {
        yield return new WaitForEndOfFrame();
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        byte[] bytes = tex.EncodeToJPG();
        TakeScreenshotServerRPC(bytes,transform.position,transform.rotation);
    }

    [Rpc(SendTo.Server)]
    private void TakeScreenshotServerRPC(byte[] PNG,Vector3 position, Quaternion rotation)
    {
        TakeScreenshotClientRPC(PNG,position, rotation);
    }

    [Rpc(SendTo.ClientsAndHost)] 
    private void TakeScreenshotClientRPC(byte[] PNG,Vector3 position, Quaternion rotation)
    {
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(PNG);
        GameObject newPhoto = Instantiate(photoPrefab,position,rotation);
        newPhoto.GetComponent<Renderer>().material.mainTexture = tex;
    }
    
    public void TakeScreenshot(InputAction.CallbackContext context)
    {
        if(IsOwner && !Menu.activeSelf && context.started)
        {
            StartCoroutine(Screenshot(Screen.width, Screen.height));
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