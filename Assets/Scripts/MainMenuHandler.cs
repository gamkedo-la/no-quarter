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
    [SerializeField] private GameObject topPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button resetSaveButton;

    private void Awake()
    {
        // todo: ask for confirmation first
        resetSaveButton.onClick.AddListener(() => GameManager.Instance.ResetSaveFile());
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

}
