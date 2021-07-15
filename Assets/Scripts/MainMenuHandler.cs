using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject topPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject topMenuInitialElement;
    [SerializeField] private GameObject settingsPanelInitialElement;
    [SerializeField] private GameObject creditsPanelInitialElement;
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button resetSaveButton;
    [SerializeField] private Button saveSettingsButton;
    [SerializeField] private Button closeSettingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button closeCreditsButton;
    [Header("Values")]
    [SerializeField] private Slider volume;
    [SerializeField] private Slider screenShake;
    [Header("Components")]
    [SerializeField] private AudioMixer audioMixer;

    private float baseVolume;

    private GameObject previousMenuSelection;

    private const string VOLUME_PREF_KEY = "volume";
    private const string SCREENSHAKE_PREF_KEY = "screen_shake";

    private void Awake()
    {
        // todo: ask for confirmation first
        resetSaveButton.onClick.AddListener(() => GameManager.Instance.ResetSaveFile());

        playButton.onClick.AddListener(() => OnPlayButtonClick());
        settingsButton.onClick.AddListener(() => OnSettingsButtonClick());
        exitButton.onClick.AddListener(() => OnExitButtonClick());

        closeSettingsButton.onClick.AddListener(() =>
        {
            LoadSettings();
            CloseSettingsMenu();
        });

        saveSettingsButton.onClick.AddListener(() =>
        {
            SaveSettings();
            CloseSettingsMenu();
        });

        creditsButton.onClick.AddListener(() => OpenCredits());
        closeCreditsButton.onClick.AddListener(() => CloseCredits());

        volume.onValueChanged.AddListener(AdjustVolume);
    }

    private void Start()
    {
        audioMixer.GetFloat("master_volume", out baseVolume);
        LoadSettings();
    }

    public void OnPlayButtonClick() 
    {
        SceneManager.LoadScene("Scenes/HoldingCell", LoadSceneMode.Single);
    }

    private void AdjustVolume(float level)
    {
        var volumeAdjust = level == 0 ? -80f : 10 * Mathf.Log10(level);
        var adjustedVolume = Mathf.Clamp(baseVolume + volumeAdjust, -80f, 0f); // ensure bad math isn't exploding eardrums.
        audioMixer.SetFloat("master_volume", adjustedVolume);
    }

    public void CloseSettingsMenu()
    {
        topPanel.SetActive(true);
        settingsPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(previousMenuSelection);
    }

    public void OnSettingsButtonClick()
    {
        var eventSystem = EventSystem.current;
        previousMenuSelection = eventSystem.currentSelectedGameObject;
        topPanel.SetActive(false);
        settingsPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(settingsPanelInitialElement);
    }

    private void OpenCredits()
    {
        var eventSystem = EventSystem.current;
        previousMenuSelection = eventSystem.currentSelectedGameObject;
        topPanel.SetActive(false);
        creditsPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(creditsPanelInitialElement);
    }

    private void CloseCredits()
    {
        topPanel.SetActive(true);
        creditsPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(previousMenuSelection);
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
        volume.SetValueWithoutNotify(vol);
        AdjustVolume(vol);

        var ss = PlayerPrefs.GetFloat(SCREENSHAKE_PREF_KEY, 1f);
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
