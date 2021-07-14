using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
            if (Application.platform == RuntimePlatform.WebGLPlayer) {
                SceneManager.LoadScene(0);
            } else {
                Application.Quit();
            }
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
