using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRangeAI : MonoBehaviour, IEnemyCapacity
{
    [SerializeField] Projectile projectile;
    [SerializeField] Transform topPart = null;
    [SerializeField] Transform leftGun = null;
    [SerializeField] Transform rightGun = null;
    [SerializeField] float timeBeforeShootingOtherGun = 0.2f;

    private NavMeshAgent navMeshAgent;
    private Enemy baseCapacity;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        baseCapacity = GetComponent<Enemy>();
    }
    public void Attack()
    {
        StartCoroutine(AIThink());
    }

    private IEnumerator AIThink()
    {
        while (baseCapacity.IsPlayerLocated())
        {
            baseCapacity.ChasePlayer();
            Fire(transform.forward);
            yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 2.5f));
        }
    }

    private void Fire(Vector3 fireDirection)
    {
        var fireDirections = GetProjectileDirections(fireDirection);

        StartCoroutine(FireBothGuns(fireDirections));
    }

    private IEnumerator FireBothGuns(List<Vector3> fireDirections)
    {
        FireOneGun(fireDirections, leftGun.position);

        yield return new WaitForSeconds(timeBeforeShootingOtherGun);
        
        FireOneGun(fireDirections, rightGun.position);
    }

    private void FireOneGun(List<Vector3> fireDirections, Vector3 gunPosition)
    {
        foreach (var dir in fireDirections)
        {
            var projectileInstance = Instantiate(projectile, gunPosition, topPart.rotation);
            var projectileComponent = projectileInstance.GetComponent<Projectile>();
            projectileComponent.projectileDirection = dir;
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
                var skewedDirection = originalDirection + (spread.x * topPart.right) + (spread.y * topPart.up);
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
