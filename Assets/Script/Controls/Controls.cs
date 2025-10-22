using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Controls : NetworkBehaviour
{
    Camera cam;
    [SerializeField] float movementSpeed=1;
    [SerializeField] float rotationSpeed=1;
    Movements movements;
    void Start()
    {
        cam = transform.GetChild(0).GetComponent<Camera>();
        movements = GetComponent<Movements>();
        movements.cam = cam;
    }


    public void Move(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            movements.enabled = true;
            movements.movement=new Vector3(moveInput.x,0,moveInput.y)*movementSpeed*Time.deltaTime;;
        
            
        }

        if (context.performed)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            movements.movement=new Vector3(moveInput.x,0,moveInput.y)*movementSpeed*Time.deltaTime;
        }

        if (context.canceled)
        {
            movements.enabled = false;
        }
        
    }

    public void Rotate(InputAction.CallbackContext context)
    {
        Vector2 mouseMovement=context.ReadValue<Vector2>();
        Vector3 actualRotation=cam.transform.localRotation.eulerAngles;
        Vector3 newRotation=actualRotation+(new Vector3(-mouseMovement.y,mouseMovement.x,0)*rotationSpeed*Time.deltaTime);
        cam.transform.localRotation=Quaternion.Euler(newRotation);
    }
    
}