using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfWebGL : MonoBehaviour
{
    void Awake()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer) {
            Debug.Log("webGL detected, hiding " +gameObject.name);
            gameObject.SetActive(false);
        }
    }
}
