using System;
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
    private float healthWarningThreashold;
    private float fillAmount = 1;
    private float lerpTime = 0.3f;
    public Image hpBarInner;
    public TextMeshProUGUI hpText;

    public Color mainColor;
    public Color warningColor;

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

        if (playerStats)
        {
            currentHealth = playerStats.currentHealth;
            maxHealth = playerStats.maxHealth;
            healthWarningThreashold = playerStats.lowHealthThreshold;
        }
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

    private void UpdateHealth(float newHealth, float delta)
    {
        StartCoroutine(HealthBarLerp(newHealth, lerpTime));
        currentHealth = newHealth;
        if (currentHealth < healthWarningThreashold)
            hpBarInner.color = warningColor;
        else 
            hpBarInner.color = mainColor;
        UpdateHealthText();
    }

    private IEnumerator HealthBarLerp(float newHealth, float lerpDuration) {
        StopCoroutine("HealthBarLerp");
        float timeElapsed = 0;
        float startHealth = currentHealth;

        while (timeElapsed < lerpDuration)
        {
            hpBarInner.fillAmount = Mathf.Lerp(startHealth, newHealth, timeElapsed / lerpDuration) / maxHealth;
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        hpBarInner.fillAmount = newHealth / maxHealth;
    }
}
