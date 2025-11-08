using Unity.Netcode;
using Unity.Services.Vivox;
using Unity.VisualScripting;
using UnityEngine;

public class ChatAudioUpdate : NetworkBehaviour
{
    private float dt = 0;
    void Update()
    {
        if (IsOwner)
        {
            dt += 1;
            if (dt >= 5)
            {

                dt = 0;
                VivoxService.Instance.Set3DPosition(transform.position, transform.position, transform.forward,
                    transform.up, StaticCode.GameCode);

            }
        }

    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        VivoxService.Instance.LeaveAllChannelsAsync();
        
    }
}
