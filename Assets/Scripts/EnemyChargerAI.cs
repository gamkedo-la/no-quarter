using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChargerAI : MonoBehaviour, IEnemyCapacity
{
    [Header("Distances")]
    [SerializeField] float chargeDistance = 10.0f;
    [Header("Speeds")]
    [SerializeField] float attackSpeed = 10.0f;
    [SerializeField] float attackAcceleration = 50.0f;
    [SerializeField] float chargeSpeed = 50.0f;
    [SerializeField] float chargeAcceleration = 100.0f;

    [Header("Durations")]
    [SerializeField] float pauseBeforeCharge = 0.1f;

    private bool isPlayerInChargeArea = false;
    private float thinkDuration = 0.5f;

    private bool isCharging = false;
    [SerializeField] float damage = 10.0f;

    private NavMeshAgent navMeshAgent;
    private Enemy baseCapacity;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        baseCapacity = GetComponent<Enemy>();
    }

    public void Attack()
    {
        StartCoroutine(AIThink());
    }

    IEnumerator AIThink() 
    {
        SetAgentSpeed();
        while (baseCapacity.IsPlayerLocated()) 
        {
            // Time before updating status
            thinkDuration = Random.Range(0.5f, 2.0f);
            DashOnPlayer();

            // baseCapacity.ChasePlayer();
            // SetAgentSpeed();
            // ProcessBehaviourIfPlayerInChargeArea();

            yield return new WaitForSeconds(thinkDuration);
        }
    }

    private void DashOnPlayer()
    {
        Transform playerLocation = baseCapacity.GetPlayerLocation();
        Vector3 playerPosition = playerLocation.position;
        
        Vector3 directionToPlayer = playerPosition - transform.position;
        directionToPlayer.Normalize();

        Vector3 potentialTarget = playerPosition + 10 * directionToPlayer;

        NavMeshHit navHit;
        NavMesh.SamplePosition(potentialTarget, out navHit, 10, NavMesh.AllAreas);

        navMeshAgent.SetDestination(navHit.position);
    }

    private void SetAgentSpeed()
    {
        navMeshAgent.acceleration = attackAcceleration;
        navMeshAgent.speed = attackSpeed;

        // if (!isPlayerInChargeArea)
        // {
        //     navMeshAgent.acceleration = attackAcceleration;
        //     navMeshAgent.speed = attackSpeed;
        // }
        // else
        // {
        //     navMeshAgent.acceleration = chargeAcceleration;
        //     navMeshAgent.speed = chargeSpeed;
        // }
    }

    private void ProcessBehaviourIfPlayerInChargeArea()
    {
        if (!isPlayerInChargeArea) { return; }

        if (isPlayerInChargeArea)
        {
            if (!isCharging)
            {
                isCharging = true;

                // Pause a little before charging
                navMeshAgent.isStopped = true;
                thinkDuration = pauseBeforeCharge;
            }
            else
            {
                navMeshAgent.isStopped = false;
            }
        }
    }

    private void CheckIfPlayerInChargeArea()
    {
        if(baseCapacity.GetDistanceToPlayer() < chargeDistance)
        {
            isPlayerInChargeArea = true;
        }
        else
        {
            isPlayerInChargeArea = false;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            other.SendMessage("TakeDamage", damage);
        }
    }

    // private void ShowPathIfDebugMode()
    // {
    //     if (!debug || !navMeshAgent.hasPath) { return; }

    //     lineRenderer.positionCount = navMeshAgent.path.corners.Length;
    //     lineRenderer.SetPositions(navMeshAgent.path.corners);
    //     lineRenderer.enabled = true;
    // }

}
