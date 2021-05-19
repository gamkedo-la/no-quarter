using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{

    [SerializeField] float healthRestoreAmount = 5.0f;
    PlayerStatsManager playerStats;

    void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatsManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerStats.RestoreHealth(healthRestoreAmount);

            //TODO: healthpacks object pooling?
            Destroy(gameObject);
        }
    }

}
