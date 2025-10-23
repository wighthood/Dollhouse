using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controls : NetworkBehaviour
{
    Camera cam;
    [SerializeField] float movementSpeed=1;
    [SerializeField] float rotationSpeed=1;
    Movements movements;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkPostSpawn();
        cam = transform.GetChild(0).GetComponent<Camera>();
        movements = GetComponent<Movements>();
        movements.cam = cam;
        if (!IsOwner)
        {
            cam.enabled = false;
        }
    }


    public void Move(InputAction.CallbackContext context)
    {
        if (!IsOwner)
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
        if (!IsOwner)
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
        Vector3 newRotation=actualRotation+(new Vector3(-input.y,input.x,0)*rotationSpeed*Time.deltaTime);
        cam.transform.localRotation=Quaternion.Euler(newRotation);
    }

}