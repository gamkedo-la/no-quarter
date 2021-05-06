using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    [SerializeField]
    private Transform playerCamera;

    [SerializeField]
    private Transform parentPortal;

    [SerializeField]
    private Transform sisterPortal;

    [SerializeField]
    private Camera camera;

    [SerializeField]
    private Material cameraMat;
    void Start ()
    {
        camera.targetTexture.Release();
        camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cameraMat.mainTexture = camera.targetTexture;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 playerOffset = playerCamera.position - sisterPortal.position;
        transform.position = parentPortal.position + playerOffset;

        float angleDifference = Quaternion.Angle(parentPortal.rotation, sisterPortal.rotation);
        Quaternion rotationDifference = Quaternion.AngleAxis(angleDifference, Vector3.up);
        Vector3 cameraDirection = rotationDifference * playerCamera.forward;

        transform.rotation = Quaternion.LookRotation(cameraDirection, Vector3.up);
    }
}
