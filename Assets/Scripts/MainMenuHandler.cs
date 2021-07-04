using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject topPanel;
    [SerializeField] private GameObject settingsPanel;

    public void OnPlayButtonClick() 
    {
        SceneManager.LoadScene("Scenes/Main", LoadSceneMode.Single);
    }
    public void OnSettingsButtonClick() 
    {
        topPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    public void OnExitButtonClick() 
    {
        Debug.Log("Quiting Application");
        Application.Quit();
    }

}
