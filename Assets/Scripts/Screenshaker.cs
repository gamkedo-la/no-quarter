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

    public void SetShakeAmountAndDemo(float amount)
    {
        shakeSize = amount;
        screenShakeNow();
    }

    private void maybeScreenshake(float currentHealth, float delta)
    {
        if (delta < 0)
        {
            screenShakeNow();
        }
    }

    public void screenShakeNow() {
        if (shakeTimeLeft <= 0f) {
            startLocation = shakeThisCamera.transform.localPosition;
        }

        if (!shakeThisCamera) return;
        //Debug.Log("Screenshake starting!");

        shakeTimeLeft = shakeTimespan;
    }

    private void Start()
    {
        shakeSize = PlayerPrefs.GetFloat(MainMenuHandler.SCREENSHAKE_PREF_KEY, 1.0f);
        shakeThisCamera = GetComponent<Camera>();
        startLocation = shakeThisCamera.transform.localPosition;
        PlayerStatsManager.OnHealthChange += maybeScreenshake;
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
            shakeThisCamera.transform.localPosition = startLocation;
            shakeTimeLeft = 0f;
        }
    }
}
