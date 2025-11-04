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
        if (playerCam == null) return;
        angle = Vector3.Angle(playerCam.transform.forward,-direction.normalized);
        if (!IsViewObstructed() &&dt < disappearTimer && angle < maxAngle && angle > minAngle)
        {
            dt += Time.deltaTime; 
            CornerViewServerRpc(true);
        }
        else
        {
            CornerViewServerRpc(false);
            dt = 0;
        }
    }

    [Rpc(SendTo.Server)]
    private void CornerViewServerRpc(bool value)
    {
        CornerViewClientRpc(value);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void CornerViewClientRpc(bool value)
    {
        meshRenderer.enabled = value;
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    internal void ResetStateClientRpc()
    {
        dt = 0;
        meshRenderer.enabled = false;
        wasSeen = false;
        vpPos = Vector3.zero;
    }

    internal bool IsViewObstructed()
    {
        vpPos = playerCam.WorldToViewportPoint(transform.position);
        maxAngle = playerCam.fieldOfView;
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
