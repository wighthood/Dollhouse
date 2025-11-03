using Unity.Netcode;
using UnityEngine;

public class PeriphericalDisappear : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    [SerializeField] private float PeriphAngle=10;
    [SerializeField] private float disappearTimer=1;
    private float dt=0;
    private Vector3 vpPos;
    private Camera playerCam;
    private CapsuleCollider playerCollider;
    private RaycastHit hit;
    private Vector3 direction;
    private MeshRenderer meshRenderer;
    private bool wasSeen = false;
    private float angle = 180;
    private float maxAngle=60;
    private float minAngle=30;

    // Update is called once per frame
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        angle = Vector3.Angle(playerCam.transform.forward,direction.normalized);
        Debug.Log(angle); 
        if (!IsViewObstructed() &&dt < disappearTimer)
        {
            if (angle < maxAngle && angle > minAngle)
            {
                meshRenderer.enabled = true;
            }
        }
        else
        {
            meshRenderer.enabled = false;
            dt = 0;
        }
        dt += Time.deltaTime;
    }

    internal void ResetState()
    {
        dt = 0;
        meshRenderer.enabled = false;
        wasSeen = false;
        vpPos = Vector3.zero;
    }

    internal bool IsViewObstructed()
    {
        vpPos = playerCam.WorldToViewportPoint(transform.position);
        maxAngle = playerCam.fieldOfView/2;
        minAngle = maxAngle - PeriphAngle;
        direction = playerCam.transform.position - transform.position;
        Physics.Raycast(transform.position + direction*GetComponent<Collider>().bounds.size.magnitude
            ,playerCam.transform.position-transform.position, out hit,
            100);
        return hit.collider != playerCollider;
    }

    internal void GetPlayerInfo(GameObject player)
    {
        playerCam = player.GetComponent<Camera>();
        playerCollider = player.GetComponent<CapsuleCollider>();
    }
}
