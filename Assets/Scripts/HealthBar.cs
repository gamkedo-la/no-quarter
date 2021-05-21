using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{   
    private PlayerStatsManager playerStats;
    private float currentHealth;
    private float fillAmount = 1;
    public Image hpBarInner;
    public TextMeshProUGUI hpText;

    private void Start() {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatsManager>();
        currentHealth = playerStats.currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBarFill();
        UpdateHealthText();
    }

    private void UpdateHealthBarFill() {
        if (currentHealth != playerStats.currentHealth) {
            currentHealth = playerStats.currentHealth;
            fillAmount = ((currentHealth * 100) / playerStats.maxHealth)/100;
            hpBarInner.fillAmount = fillAmount;
        }
    }

    private void UpdateHealthText() {
        hpText.text = Mathf.Round(currentHealth).ToString();
    }
}
