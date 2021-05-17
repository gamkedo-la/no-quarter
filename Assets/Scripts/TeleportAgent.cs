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
            canTeleport = false;
            this.toPortal = toPortal;
            transform.position = pos;
            //transform.rotation = rot;
            Debug.Log("Teleport");
            StartCoroutine("WaitToTeleport");
        }
    }
    public IEnumerator WaitToTeleport()
    {
       yield return new WaitForSeconds(0.1f);
        canTeleport = true;
    }
   
}
