using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRangeAI : MonoBehaviour
{
    [SerializeField] Projectile projectile;
    [SerializeField] Transform firePosition = null;

    private GameObject player;
    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();

        StartCoroutine(Think());
    }
    private IEnumerator Think()
    {
        while (true) {
            Fire(transform.forward);
            yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 2.5f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        navMeshAgent.SetDestination(player.transform.position);
    }

    private void Fire(Vector3 fireDirection)
    {
        var fireDirections = GetProjectileDirections(fireDirection);

        foreach (var dir in fireDirections)
        {
            var projectileInstance = Instantiate(projectile, firePosition.position, player.transform.rotation);
            var projectileComponent = projectileInstance.GetComponent<Projectile>();
            projectileComponent.projectileDirection = dir;
            // projectileComponent.originator = this;
        }
    }

    List<Vector3> GetProjectileDirections(Vector3 originalDirection)
    {
        var numProjectiles = GetProjectileCount();
        List<Vector3> directions = new List<Vector3>();

        if (numProjectiles > 1)
        {
            for (var i = 0; i < numProjectiles; i++)
            {
                Vector3 spread = UnityEngine.Random.insideUnitCircle * 0.1f;
                var skewedDirection = originalDirection + (spread.x * player.transform.right) + (spread.y * player.transform.up);
                directions.Add(skewedDirection);
            }
        }
        else
        {
            directions.Add(originalDirection);
        }

        return directions;
    }

    int GetProjectileCount()
    {
        var projectileCount = projectile.baseProjectileCount;
        return projectileCount;
    }
}
