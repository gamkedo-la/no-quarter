using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportAgent : MonoBehaviour
{
    private Transform toPortal;
    public bool canTeleport = true;
    public virtual void Teleport (Transform toPortal, Vector3 pos, Quaternion rot)
    {
        if(canTeleport)
        {
            this.toPortal = toPortal;
            transform.position = pos;
            transform.rotation = rot;
            canTeleport = false;
        }
    }
    public virtual void LateUpdate()
    {
        if (toPortal && canTeleport == false)
        {
            if (Vector3.Distance(gameObject.transform.position, toPortal.position) > 2f)
            {
                canTeleport = true;
            }
        }
    }
}
