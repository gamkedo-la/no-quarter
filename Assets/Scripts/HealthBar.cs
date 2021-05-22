using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{   
    private PlayerStatsManager playerStats;
    private float currentHealth;
    private float maxHealth;
    private float fillAmount = 1;
    public Image hpBarInner;
    public TextMeshProUGUI hpText;

    private void OnEnable()
    {
        PlayerStatsManager.OnHealthChange += UpdateHealth;
    }

    private void OnDisable() 
    {
        PlayerStatsManager.OnHealthChange -= UpdateHealth;
    }

    private void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatsManager>();
        currentHealth = playerStats.currentHealth;
        maxHealth = playerStats.maxHealth;
    }

    private void UpdateHealthBarFill()
    {
            fillAmount = currentHealth / maxHealth;
            hpBarInner.fillAmount = fillAmount;
    }

    private void UpdateHealthText()
    {
        hpText.text = Mathf.Round(currentHealth).ToString();
    }

    private void UpdateHealth(float newHealth)
    {
        currentHealth = newHealth;
        UpdateHealthBarFill();
        UpdateHealthText();
    }
}
