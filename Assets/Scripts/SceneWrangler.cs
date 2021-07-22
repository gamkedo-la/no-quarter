using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneWrangler : MonoBehaviour
{
    public string loadingScene;
    public GameObject loadingScreen;
	public static SceneWrangler Instance;
    public float minLoadingTime = 3f;

    private AsyncOperation loadingOperation;

    public List<string> scenes;
    public List<AudioClip> sceneTracks;

	void Awake() {
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}
		else Destroy(gameObject);
	}

    public void LoadScene(string sceneName)
    {
        loadingScreen.SetActive(true);
        loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        loadingOperation.completed += (operation) => loadingScreen.SetActive(false);
        loadingOperation.completed += (operation) =>
        {
	        loadingScreen.SetActive(false);

	        var index = scenes.FindIndex(s => s == sceneName);
	        if (index >= 0)
	        {
		        var clip = sceneTracks[index];
				AudioManager.Instance.SetTrack(clip);
	        }
        };
    }
}
