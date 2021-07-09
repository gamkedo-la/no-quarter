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
    private GameObject loadingScreenInstance;

	void Awake() {
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
		DontDestroyOnLoad(this);
	}

    // Update is called once per frame.
    void Update()
    {
        if (loadingOperation != null)
        {
            if (loadingOperation.isDone)
            {
                Destroy(loadingScreenInstance);
            }
            // else
            // {
            //     var percentDone = Mathf.Round(Mathf.Clamp01(loadingOperation.progress / 0.9f) * 100);
            // }
        }
        else
        {
            Destroy(loadingScreenInstance);
        }
    }

    bool OperationCompleted()
    {
        return loadingOperation == null || loadingOperation.isDone;
    }

    public void LoadScene(string sceneName)
    {
        loadingScreenInstance = Instantiate(loadingScreen, transform.root);
        loadingOperation = SceneManager.LoadSceneAsync(sceneName);
    }
}
