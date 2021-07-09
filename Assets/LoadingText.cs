using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class LoadingText : MonoBehaviour
{
    public TMP_Text loadingTextObject;
    public string baseText = "Loading";
    public int maxDots = 3;
    public float timeBetweenDotChange = 0.333f;
    private string[] loadingMessages;
    private int currentMessage = 0;

    private PlayerInputHandler[] players;

    // Start is called before the first frame update
    void Start()
    {
        players = FindObjectsOfType<PlayerInputHandler>();

        if (players.Length > 0)
        {
            foreach (var player in players)
            {
                player.enabled = false;
            }
        }

        loadingMessages = new string[maxDots+1];
        loadingMessages[0] = baseText;
        for (var i = 1; i < loadingMessages.Length; i++)
        {
            loadingMessages[i] = baseText + new string('.', i);
        }

        StartCoroutine(UpdateLoadingText());
    }

    // private void OnDestroy()
    // {
    //     if (players.Length > 0)
    //     {
    //         foreach (var player in players)
    //         {
    //             if (player != null) player.enabled = true;
    //         }
    //     }
    // }

    IEnumerator UpdateLoadingText()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenDotChange);
            currentMessage = (currentMessage + 1) % (loadingMessages.Length);
            loadingTextObject.text = loadingMessages[currentMessage];
        }
    }
}
