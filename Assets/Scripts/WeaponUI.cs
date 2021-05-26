using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{   
    public GameObject reticule;

    void OnEnable()
    {
        reticule.SetActive(true);
    }
    void OnDisable()
    {
        reticule.SetActive(false);
    }
}
