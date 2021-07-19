using TMPro;
using UnityEngine;

public class LoadingText : MonoBehaviour
{
    public TMP_Text loadingTextObject;
    public string baseText = "Loading";
    public int maxDots = 3;
    public float timeBetweenDotChange = 0.333f;
    private string[] loadingMessages;
    private int currentMessage = 0;
    private float timeSinceChange;

    // Start is called before the first frame update
    void Start()
    {
        loadingMessages = new string[maxDots+1];
        loadingMessages[0] = baseText;
        for (var i = 1; i < loadingMessages.Length; i++)
        {
            loadingMessages[i] = baseText + new string('.', i);
        }

    }

    private void Update()
    {
        timeSinceChange += Time.deltaTime;
        if (timeSinceChange >= timeBetweenDotChange)
        {
            currentMessage = (currentMessage + 1) % (loadingMessages.Length);
            loadingTextObject.text = loadingMessages[currentMessage];
            timeSinceChange = 0f;
        }
    }
}
