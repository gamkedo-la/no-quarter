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
    [SerializeField] AudioClip deathSFX = null;

    [Header("Distances")]
    [SerializeField] float detectionDistance = 20.0f;
    [SerializeField] float minWanderDistance = 5.0f;
    [SerializeField] float maxWanderDistance = 15.0f;

    [Header("Speeds")]
    [SerializeField] float wanderSpeed = 5.0f;
    [SerializeField] float wanderAcceleration = 10.0f;
    [SerializeField] float rotationSpeed = 10.0f;

    [Header("Durations")]
    [SerializeField] float redAlertDuration = 5.0f;
    [SerializeField] float minWanderDuration = 3.0f;
    [SerializeField] float minTimeBeforeLosingPlayer = 1.5f;

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
    private float timeTillNextWander = 0.0f;

    private IEnemyCapacity capacity = null;

    private float timeSincePlayerLastSeen = 0f;

    public delegate void Death();
    public static event Death OnDeath;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = wanderSpeed;
        player = GameObject.FindGameObjectWithTag("Player");
        capacity = GetComponent<IEnemyCapacity>();

        StartCoroutine(AIThink());
    }

    private void Update()
    {
        if (IsPlayerLocated())
        {
            RotateTowardsPlayer();
        }
        
        timeTillNextWander -= Time.deltaTime;
    }

    private void RotateTowardsPlayer()
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = player.transform.position - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = rotationSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }

        StartCoroutine(EnterRedAlertMode());
    }

    private void Die()
    {
        ParticleSystem deathFXClone = Instantiate(deathFX, transform.position, transform.rotation);
        OnDeath?.Invoke();
        AudioSource.PlayClipAtPoint(deathSFX, transform.position);

        Destroy(deathFXClone.gameObject, 2);
        Destroy(this.gameObject);
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
            timeSincePlayerLastSeen = 0f;

            if (!isPlayerLocated)
            {
                isPlayerLocated = true;
                ChasePlayer();
                capacity.Attack();
            }
        }
        else if (isPlayerLocated)
        {
            timeSincePlayerLastSeen += Time.deltaTime;

            if (timeSincePlayerLastSeen > minTimeBeforeLosingPlayer)
            {
                isPlayerLocated = false;
            }
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
        
        if (navMeshAgent.isStopped) { navMeshAgent.isStopped = false; }
        navMeshAgent.SetDestination(target);
    }

    public void ChasePlayer()
    {
        target = player.transform.position;
        
        if (navMeshAgent.isStopped) { navMeshAgent.isStopped = false; }
        navMeshAgent.SetDestination(target);
    }

    private void RandomWander()
    {
        if (isPlayerLocated){ return; }
        if (timeTillNextWander > 0) { return; }

        target = transform.position;
        
        for(int i = 0 ; i < 100 ; ++i)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * maxWanderDistance;
            target += randomDirection; // += transform.position;
            NavMeshHit navHit;
            NavMesh.SamplePosition(target, out navHit, maxWanderDistance, NavMesh.AllAreas);
            target = navHit.position;

            if(Vector3.Distance(transform.position, target) > minWanderDistance) { break; }
        }
        
        timeTillNextWander = minWanderDuration;
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

    public Transform GetPlayerLocation()
    {
        return player.transform;
    }
}
