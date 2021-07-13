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
    }
}
