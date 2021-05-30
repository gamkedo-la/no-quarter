using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private void Awake() {
        PlayerInputHandler.OnPause += ToggleMenu;
    }
    private void OnDestroy() {
        PlayerInputHandler.OnPause -= ToggleMenu;
    }
    private void ToggleMenu() {
        Canvas canvas = GetComponent<Canvas>();
        canvas.enabled = !canvas.enabled;
    }

    public void SampleQuest(){
        Debug.Log("holiwis");
    }
}
