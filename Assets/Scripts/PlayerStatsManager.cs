using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    private bool paused = false;    //TODO: support game pausing -> stamina recovery halted while on Pause Screen

    public float maxHealth = 100f;
    public float lowHealthThreshold = 20f;
    public float currentHealth = 100f;

    [Header("Stamina")]
    [SerializeField] private float staminaChargeRecoveryTime = 5.0f;
    private int staminaChargeCount = 3;
    private Queue<IEnumerator> staminaChargeQueue = new Queue<IEnumerator>();
    private IEnumerator currentRecharge = null;
    public static event Action<int> OnStaminaChange;

    public List<AudioClip> heartbeats = new List<AudioClip>();
    public float heartbeatPace = 1f;
    private SfxHelper sfx;

    public delegate void HealthChange(float currentHealth);
    public static event HealthChange OnHealthChange;


    private void OnEnable() {
        PlayerInputHandler.dashStarted += UseStaminaCharge;
    }

    private void OnDisable() {
        PlayerInputHandler.dashStarted -= UseStaminaCharge;
    }

    void Start()
    {
        currentHealth = maxHealth;
        sfx = GetComponent<SfxHelper>();
        StartCoroutine(PlayHeartbeat());
        StartCoroutine(StaminaChargesCoordinator());
    }

    IEnumerator PlayHeartbeat()
    {
        while (true)
        {
            var waitTime = heartbeatPace;
            if (currentHealth < lowHealthThreshold)
            {
                sfx.PlayRandomAudioOneshot(heartbeats, 0.7f, 1.0f, 0.9f, 1.1f);
            }
            yield return new WaitForSeconds(waitTime);
        }
    }


    public void TakeDamage(float amount) {
        if (amount > 0) {
            currentHealth -= amount;
            if (currentHealth <= 0 ) {
                //TODO: player death or level fail; currently just reset hp to maxhp
                currentHealth = maxHealth;
            }
        }
        OnHealthChange?.Invoke(currentHealth);
    }

    public void RestoreHealth(float amount)
    {
        if (amount > 0)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
        OnHealthChange?.Invoke(currentHealth);
    }

    
    private int CountStaminaChargesLeft()
    {
        int chargesLeft = staminaChargeCount - staminaChargeQueue.Count;
        if (currentRecharge != null) chargesLeft--;
        return chargesLeft;
    }

    private IEnumerator StaminaChargesCoordinator()
    {
        while(true)
        {
            while (staminaChargeQueue.Count > 0)
            {   
                currentRecharge = staminaChargeQueue.Dequeue();
                yield return StartCoroutine(currentRecharge);
                currentRecharge = null;
                OnStaminaChange?.Invoke(CountStaminaChargesLeft());
            }
            yield return null;
        }
    }

    private IEnumerator RecoverStaminaCharge()
    {
        float counter = 0;
        while (counter < staminaChargeRecoveryTime)
        {
            if(!paused)
            {
            counter += Time.deltaTime;
            }
            yield return null;
        }
    }

    public void UseStaminaCharge()
    {
        staminaChargeQueue.Enqueue(RecoverStaminaCharge());
        OnStaminaChange?.Invoke(CountStaminaChargesLeft());
    }
}
