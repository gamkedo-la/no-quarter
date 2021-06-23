using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : TeleportAgent
{
    [SerializeField] private float health = 2.0f;    
    [SerializeField] ParticleSystem deathFX = null;
    [SerializeField] float viewCone = 60.0f;

    [Header("Distances")]
    [SerializeField] float detectionDistance = 20.0f;
    [SerializeField] float minWanderDistance = 5.0f;
    [SerializeField] float maxWanderDistance = 15.0f;

    [Header("Speeds")]
    [SerializeField] float wanderSpeed = 5.0f;
    [SerializeField] float wanderAcceleration = 10.0f;
    [SerializeField] float attackSpeed = 10.0f;
    [SerializeField] float attackAcceleration = 50.0f;

    [Header("Durations")]
    [SerializeField] float redAlertDuration = 5.0f;
    [SerializeField] float minWanderDuration = 3.0f;

    private GameObject player = null;
    private NavMeshAgent navMeshAgent;
    private Vector3 target;
    private RaycastHit rayToPlayer;
    private float thinkDuration = 0.5f;
    private bool isPlayerInDetectionArea = false;
    private bool isPlayerVisible = false;
    public bool isPlayerLocated = false;
    private bool isInRedAlertMode = false;
    private float distanceToPlayer;
    private float angleToPlayer;
    private float timeSinceLastWander = 0.0f;

    private IEnemyCapacity capacity = null;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = wanderSpeed;
        player = GameObject.FindGameObjectWithTag("Player");
        capacity = GetComponent<IEnemyCapacity>();

        StartCoroutine(AIThink());
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            ParticleSystem deathFXClone = Instantiate(deathFX, transform.position, transform.rotation);
            Destroy(deathFXClone.gameObject, 2);
            Destroy(this.gameObject);
        }

        StartCoroutine(EnterRedAlertMode());
    }

    IEnumerator AIThink() 
    {
        while (true) 
        {
            // Time before updating status
            thinkDuration = UnityEngine.Random.Range(0.5f, 2.0f);

            LookForPlayer();
            SetTarget();

            yield return new WaitForSeconds(thinkDuration);
        }
    }

    private void LookForPlayer()
    {
        if (isInRedAlertMode) { return; }

        CheckIfPlayerInDetectionArea();
        CheckIfPlayerIsVisible();

        if (isPlayerInDetectionArea && isPlayerVisible)
        {
            if (!isPlayerLocated)
            {
                isPlayerLocated = true;
                ChasePlayer();
                capacity.Attack();
            }
        }
        else if (isPlayerLocated)
        {
            isPlayerLocated = false;
        }
    }

    private void CheckIfPlayerIsVisible()
    {
        if (!isPlayerInDetectionArea) { return; }

        if (Physics.Linecast(transform.position, player.transform.position, out rayToPlayer))
        {
            Transform hitLocation = rayToPlayer.collider.gameObject.transform;
            
            if (hitLocation.CompareTag("Player"))
            {
                isPlayerVisible = true;
                return;
            }
        }

        isPlayerVisible = false;
    }

    private void CheckIfPlayerInDetectionArea()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        angleToPlayer = Vector3.Angle(transform.forward, player.transform.position - transform.position);

        if (distanceToPlayer < detectionDistance && angleToPlayer < viewCone)
        {
            isPlayerInDetectionArea = true;
        }
        else
        {
            isPlayerInDetectionArea = false;
        }
    }

    private void SetTarget()
    {
        if (isPlayerLocated) { return; }

        navMeshAgent.acceleration = wanderAcceleration;
        navMeshAgent.speed = wanderSpeed;

        RandomWander();
        
        navMeshAgent.SetDestination(target);
    }

    public void ChasePlayer()
    {
        target = player.transform.position;
        navMeshAgent.SetDestination(target);
    }

    private void RandomWander()
    {
        if (isPlayerLocated){ return; }
        if (Time.time - timeSinceLastWander < minWanderDuration) { return; }

        do {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * maxWanderDistance;
            randomDirection += transform.position;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randomDirection, out navHit, maxWanderDistance, NavMesh.AllAreas);
            target = navHit.position;
        } while(Vector3.Distance(transform.position, target) < minWanderDistance);
        
        timeSinceLastWander = Time.time;
    }

    private IEnumerator EnterRedAlertMode()
    {
        isInRedAlertMode = true;
        isPlayerLocated = true;
        yield return new WaitForSeconds(redAlertDuration);

        isInRedAlertMode = false;
    }

    public float GetDistanceToPlayer()
    {
        return distanceToPlayer;
    }

    public bool IsPlayerLocated()
    {
        return isPlayerLocated;
    }
}
