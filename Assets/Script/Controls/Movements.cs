using Unity.Netcode;
using UnityEngine;

public class Movements : NetworkBehaviour
{
    internal Vector3 movement;
    internal Camera cam;

    // Update is called once per frame
    void Update()
    {
        Vector3 horizontalForward=new Vector3(cam.transform.forward.x,0,cam.transform.forward.z);
        Vector3 horizontalRight=new Vector3(cam.transform.right.x,0,cam.transform.right.z);
        transform.position+=horizontalForward*movement.z;
        transform.position+=horizontalRight*movement.x;
    }
}