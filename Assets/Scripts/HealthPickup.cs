using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour {

    [SerializeField] float healthRestoreAmount = 5.0f;
    PlayerStatsManager playerStats;

    void Start() {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatsManager>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            //TODO: Move health restore logic into PlayerStatsManager
            float newHealth;
            newHealth = playerStats.currentHealth + healthRestoreAmount;
            if (newHealth >= playerStats.maxHealth)
                playerStats.currentHealth = playerStats.maxHealth;
            else playerStats.currentHealth = newHealth;

            //TODO: healthpacks object pooling?
            Destroy(gameObject);
        }
    }
}
