using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// really simple screenshake by mcf
// note: this causes the camera to drift permanently 
// if its local position is not reset by something else
// (such as follow camera scripts etc)

public class Screenshaker : MonoBehaviour
{
    private Camera shakeThisCamera;
    
    public float shakeSize = 1f;
    public float shakeTimespan = 0.25f;
    public float shakeTimeLeft = 0f;

    private Vector3 startLocation;

    public void screenShakeNow() {
        if (!shakeThisCamera) return;
        Debug.Log("Screenshake starting!");
        shakeTimeLeft = shakeTimespan;
        startLocation = shakeThisCamera.transform.localPosition;
    }

    private void Start() {
        shakeThisCamera = GetComponent<Camera>();
        Debug.Log("Screenshake is enabled.");
    }

    private void Update() {
        if (shakeThisCamera != null && shakeTimeLeft > 0f) {
            shakeThisCamera.transform.localPosition = startLocation + Random.insideUnitSphere * shakeSize;
            shakeTimeLeft -= Time.deltaTime;
            if (shakeTimeLeft<0f) { // reset to avoid drift
                
                // this might not be desired!!!
                Debug.Log("Screenshake has completed. Resetting camera position...");
                shakeThisCamera.transform.localPosition = startLocation;

            }
        } else {
            shakeTimeLeft = 0f;
        }
    }
}
