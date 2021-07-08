using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRangeAI : MonoBehaviour, IEnemyCapacity
{
    [SerializeField] Transform topPart = null;
    [SerializeField] ParticleSystem leftGun = null;
    [SerializeField] ParticleSystem rightGun = null;
    [SerializeField] float timeBeforeShootingOtherGun = 0.2f;
    [SerializeField] float minDistanceToShoot = 5.0f;
    [SerializeField] float maxDistanceToShoot = 15.0f;
    [SerializeField] float shotDamage = 5.0f;
    [SerializeField] AudioClip laserSFX = null;

    private NavMeshAgent navMeshAgent;
    private Enemy baseCapacity;
    private AudioSource audioSource;

    private float shotDuration = 0f;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        baseCapacity = GetComponent<Enemy>();
        audioSource = GetComponent<AudioSource>();

        shotDuration = leftGun.main.duration;
    }

    private void Update()
    {
        if (!baseCapacity.IsPlayerLocated()) {return;}

        if (CheckIfInShootingDistance())
        {
            navMeshAgent.isStopped = true;
        }

    }

    private bool CheckIfInShootingDistance()
    {
        return (baseCapacity.GetDistanceToPlayer() > minDistanceToShoot && 
                baseCapacity.GetDistanceToPlayer() < maxDistanceToShoot);
    }

    public void Attack()
    {
        StartCoroutine(AIThink());
    }

    private IEnumerator AIThink()
    {
        while (baseCapacity.IsPlayerLocated())
        {
            RemainWithinShootingRange();

            Fire();

            float timeBeforeNextShot = shotDuration + UnityEngine.Random.Range(0.5f, 1.5f);

            yield return new WaitForSeconds(timeBeforeNextShot);
        }
    }

    private void RemainWithinShootingRange()
    {
        if (baseCapacity.GetDistanceToPlayer() > maxDistanceToShoot)
        {
            baseCapacity.ChasePlayer();
        }
        else if (baseCapacity.GetDistanceToPlayer() < minDistanceToShoot)
        {
            Vector3 playerPosition = baseCapacity.GetPlayerLocation().position;
            Vector3 directionToPlayer = playerPosition - transform.position;
            directionToPlayer.Normalize();

            Vector3 destination = playerPosition - minDistanceToShoot * directionToPlayer;

            if (navMeshAgent.isStopped) { navMeshAgent.isStopped = false; }
            navMeshAgent.SetDestination(destination);
        }
    }

    private void Fire()
    {
        StartCoroutine(FireBothGuns());
    }

    private IEnumerator FireBothGuns()
    {
        FireOneGun(leftGun);

        yield return new WaitForSeconds(timeBeforeShootingOtherGun);
        
        FireOneGun(rightGun);
    }

    private void FireOneGun(ParticleSystem gun)
    {
        // gun.transform.LookAt(baseCapacity.GetPlayerLocation());
        gun.Play();
        audioSource.PlayOneShot(laserSFX);
    }

    public float GetDamageAmount()
    {
        return shotDamage;
    }
}
