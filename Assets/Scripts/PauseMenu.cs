using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Interactables")]
    public Slider volumeSlider;
    public Slider screenShakeSlider;
    public Toggle immortalMode;
    public Button saveGameButton;
    public Button exitArenaButton;
    public Button exitTitleButton;
    [Header("Asset references")]
    public AudioMixer audioMixer;

    private float baseVolume;

    public static string IMMORTAL_MODE_KEY = "immortal_mode";

    public delegate void Pause();
    public static event Pause OnPause;
    private FPSPlayerControls controls;


    public delegate void ImmortalToggle(bool isOn);

    public static event ImmortalToggle OnImmortalToggle;

    private void Awake() {
        controls = new FPSPlayerControls();

        controls.UI.Pause.performed += ctx =>
        {
            ToggleMenu();
        };

        saveGameButton.onClick.AddListener(() =>
        {
            // todo: get rid of this, and make sure the game is saved when it needs to be.
            // having a button like this makes players feel unsure about whether or not
            // they need to use it, and when.
            GameManager.Instance.SaveGame();
        });

        exitArenaButton.onClick.AddListener(() =>
        {
            ToggleMenu();
            SceneWrangler.Instance.LoadScene("Scenes/HoldingCell");
        });

        exitTitleButton.onClick.AddListener(() =>
        {
            ToggleMenu();
            UnlockCursor();
            SceneWrangler.Instance.LoadScene("Scenes/MainMenu");
        });

        immortalMode.onValueChanged.AddListener((isOn) =>
        {
            // set this value on the player controller
            OnImmortalToggle?.Invoke(isOn);
            PlayerPrefs.SetInt(IMMORTAL_MODE_KEY, isOn ? 1 : 0);
            PlayerPrefs.Save();
        });

        volumeSlider.onValueChanged.AddListener(level =>
        {
            var volumeAdjust = level == 0 ? -80f : 10 * Mathf.Log10(level);
            var adjustedVolume = Mathf.Clamp(baseVolume + volumeAdjust, -80f, 0f); // ensure bad math isn't exploding eardrums.
            audioMixer.SetFloat("master_volume", adjustedVolume);
            PlayerPrefs.SetFloat(MainMenuHandler.VOLUME_PREF_KEY, level);
            PlayerPrefs.Save();
        });
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void Start()
    {
        var volume = PlayerPrefs.GetFloat(MainMenuHandler.VOLUME_PREF_KEY, 0.5f);
        volumeSlider.SetValueWithoutNotify(volume);

        var immortalModeSetting = PlayerPrefs.GetInt(IMMORTAL_MODE_KEY, 0);
        immortalMode.SetIsOnWithoutNotify(immortalModeSetting == 1);

        audioMixer.GetFloat("master_volume", out baseVolume);
        if(EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(immortalMode.gameObject);
        }

    }

    private void ToggleMenu() {
        Canvas canvas = GetComponent<Canvas>();
        canvas.enabled = !canvas.enabled;
        OnPause?.Invoke();
    }

    private void UnlockCursor()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
