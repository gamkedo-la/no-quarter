using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCollider : MonoBehaviour
{
    [SerializeField]
    private Transform sisterPortal;

    private Vector3 exitPos;
    private Quaternion exitRot;

    private void OnTriggerEnter(Collider other)
    {
       if(other.gameObject.GetComponent<TeleportAgent>())
        {
            exitPositionRotationCalculator(other.gameObject.transform);
            other.gameObject.GetComponent<TeleportAgent>().Teleport(sisterPortal, exitPos, exitRot);
        }
    }
    private void exitPositionRotationCalculator(Transform agentTransform)
    {
        Vector3 offsetFromPortal = agentTransform.position - transform.position;
        exitPos = sisterPortal.position - offsetFromPortal;

        float angleDifference = Quaternion.Angle(transform.rotation, sisterPortal.rotation);
        Quaternion rotationDifference = Quaternion.AngleAxis(angleDifference, Vector3.up);
        Vector3 agentDirection = rotationDifference * agentTransform.forward;

        exitRot = Quaternion.LookRotation(agentDirection, Vector3.up);
    }
}
