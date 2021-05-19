using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public float maxHealth = 100f;
    public float lowHealthThreshold = 20f;
    public float currentHealth = 100f;


    public List<AudioClip> heartbeats = new List<AudioClip>();
    public float heartbeatPace = 1f;
    private SfxHelper sfx;


    void Start()
    {
        currentHealth = maxHealth;
        sfx = GetComponent<SfxHelper>();
        StartCoroutine(PlayHeartbeat());
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
    }
}
