using System;
using Unity.Netcode;
using UnityEngine;

public class Movements : NetworkBehaviour
{
    internal Vector3 movement;
    internal Camera cam;
    internal Animator animator;

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", movement.magnitude);
        Vector3 horizontalForward=new Vector3(cam.transform.forward.x,0,cam.transform.forward.z);
        Vector3 horizontalRight=new Vector3(cam.transform.right.x,0,cam.transform.right.z);
        transform.position+=horizontalForward*(movement.z * Time.deltaTime);
        transform.position+=horizontalRight*(movement.x * Time.deltaTime);

    }

    private void OnDisable()
    {
        animator.SetFloat("Speed", 0);
    }
}