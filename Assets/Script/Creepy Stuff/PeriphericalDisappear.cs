using System;
using Unity.Netcode;
using UnityEngine;


public class PeriphericalDisappear : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float dt=0;
    private Vector3 vpPos;
    private Camera playerCam;
    private CapsuleCollider playerCollider;
    private RaycastHit hit;
    private MeshRenderer meshRenderer;
    private bool wasSeen = false;

    // Update is called once per frame
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        
        if (!IsViewObstructed())
        {
            if (vpPos.x >= 0f && vpPos.x <= .2f && vpPos.y >= 0f && vpPos.y <= .2f && vpPos.z > 0f && vpPos.z<100.0f)
            {
                dt += Time.deltaTime;
                if (wasSeen || dt >= .5f)
                {
                    meshRenderer.enabled = false;
                }
                
            
            }
            wasSeen = true;

        }
        else
        {
            dt = 0;
        }
        
    }

    internal void ResetState()
    {
        dt = 0;
        meshRenderer.enabled = true;
        wasSeen = false;
        vpPos = Vector3.zero;
        
    }

    bool IsViewObstructed()
    {
        vpPos = playerCam.WorldToViewportPoint(transform.position);
        Physics.Raycast(transform.position, playerCam.transform.position-transform.position, out hit, 100);
        return hit.collider!=playerCollider;
    }

    internal void GetPlayerInfo(GameObject player)
    {
        playerCam = player.GetComponent<Camera>();
        playerCollider = player.GetComponent<CapsuleCollider>();
    }
}
