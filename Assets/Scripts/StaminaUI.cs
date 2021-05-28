using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaUI : MonoBehaviour
{   
    public GameObject[] charges;


    private void OnEnable() {
        PlayerStatsManager.OnStaminaChange += UpdateStaminaUI;
    }

    private void OnDisable() {
        PlayerStatsManager.OnStaminaChange -= UpdateStaminaUI;
    }

    private void UpdateStaminaUI(int chargesAvailable)
    {
        for (int i=0; i<chargesAvailable; i++)
        {
            charges[i].SetActive(true);
        }
        for (int i=chargesAvailable; i<charges.Length; i++)
        {
            charges[i].SetActive(false);
        }
    }
}
