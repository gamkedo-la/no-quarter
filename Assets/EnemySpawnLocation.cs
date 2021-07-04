using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemySpawnLocation : MonoBehaviour
{
     [SerializeField] private float spawnInterval = 3f;
     [SerializeField] private int killsPerTier = 20;
     [SerializeField] private List<GameObject> enemyQueue;

     private List<GameObject> activeEnemies;
     private int tier = 1;
     private int killsThisTier = 0;

     private void Start()
     {
          StartCoroutine(SpawnEnemies());
          Enemy.OnDeath += KillTally;
     }

     private void KillTally()
     {
          killsThisTier++;

          if (killsThisTier >= killsPerTier)
          {
               tier = math.min(enemyQueue.Count, tier + 1);
               killsThisTier = 0;
          }
     }

     IEnumerator SpawnEnemies() {
          while (true)
          {
               for (var i = 0; i < tier; i++)
               {
                    var enemyType = enemyQueue[i];
                    Instantiate(enemyType, transform);
               }

               yield return new WaitForSeconds(spawnInterval);
          }
     }
}
