using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button ExitButton;

    private void Awake() {
        PlayerInputHandler.OnPause += ToggleMenu;

        ExitButton.onClick.AddListener(() =>
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        });
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
