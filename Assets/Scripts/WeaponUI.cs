using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{   
    public GameObject reticule;

    void OnEnable()
    {
        if (reticule != null ) reticule.SetActive(true);
    }
    void OnDisable()
    {
        if (reticule != null ) reticule.SetActive(false);
    }
}
