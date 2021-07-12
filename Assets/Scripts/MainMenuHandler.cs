using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject topPanel;
    [SerializeField] private GameObject settingsPanel;
    [Header("Buttons")]
    [SerializeField] private Button resetSaveButton;
    [SerializeField] private Button saveSettingsButton;
    [SerializeField] private Button closeSettingsButton;
    [Header("Values")]
    [SerializeField] private Slider volume;
    [SerializeField] private Slider screenShake;


    private const string VOLUME_PREF_KEY = "volume";
    private const string SCREENSHAKE_PREF_KEY = "screen_shake";

    private void Awake()
    {
        // todo: ask for confirmation first
        resetSaveButton.onClick.AddListener(() => GameManager.Instance.ResetSaveFile());

        closeSettingsButton.onClick.AddListener(() =>
        {
            LoadSettings();
            topPanel.SetActive(true);
            settingsPanel.SetActive(false);
        });

        saveSettingsButton.onClick.AddListener(() =>
        {
            SaveSettings();
            topPanel.SetActive(true);
            settingsPanel.SetActive(false);
        });
    }

    private void Start()
    {
        LoadSettings();
    }

    public void OnPlayButtonClick() 
    {
        SceneManager.LoadScene("Scenes/HoldingCell", LoadSceneMode.Single);
    }

    public void OnSettingsButtonClick() 
    {
        topPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnExitButtonClick() 
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void LoadSettings()
    {
        var vol = PlayerPrefs.GetFloat(VOLUME_PREF_KEY, 0.5f);
        var ss = PlayerPrefs.GetFloat(SCREENSHAKE_PREF_KEY, 1f);
        volume.SetValueWithoutNotify(vol);
        screenShake.SetValueWithoutNotify(ss);
    }

    private void SaveSettings()
    {
        // todo: make this logorithmic or w/e
        // todo: *actually* adjust the game's volume level based on this value
        var vol = volume.value;
        PlayerPrefs.SetFloat(VOLUME_PREF_KEY, vol);
        PlayerPrefs.SetFloat(SCREENSHAKE_PREF_KEY, screenShake.value);
        PlayerPrefs.Save();
    }

}
