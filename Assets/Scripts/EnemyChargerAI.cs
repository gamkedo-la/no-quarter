using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChargerAI : MonoBehaviour
{
    private GameObject player = null;
    [SerializeField] bool debug = false;
    [SerializeField] float viewCone = 60.0f;
 
    [Header("Distances")]
    [SerializeField] float detectionDistance = 20.0f;
    [SerializeField] float chargeDistance = 10.0f;
    [SerializeField] float minWanderDistance = 5.0f;
    [SerializeField] float maxWanderDistance = 15.0f;

    [Header("Speeds")]
    [SerializeField] float wanderSpeed = 5.0f;
    [SerializeField] float wanderAcceleration = 10.0f;
    [SerializeField] float attackSpeed = 10.0f;
    [SerializeField] float attackAcceleration = 50.0f;
    [SerializeField] float chargeSpeed = 50.0f;
    [SerializeField] float chargeAcceleration = 100.0f;

    [Header("Durations")]
    [SerializeField] float redAlertDuration = 5.0f;
    [SerializeField] float minWanderDuration = 3.0f;
    [SerializeField] float pauseBeforeCharge = 0.1f;

    private NavMeshAgent navMeshAgent;

    [SerializeField] Vector3 target;

    private bool isPlayerInDetectionArea = false;
    private bool isPlayerVisible = false;

    private bool isPlayerLocated = false;
    private bool isInRedAlertMode = false;
    private float distanceToPlayer;
    private float angleToPlayer;
    private RaycastHit rayToPlayer;
    private float timeSinceLastWander = 0.0f;
    private bool isPlayerInChargeArea = false;
    private float thinkDuration = 0.5f;

    private bool isCharging = false;
    [SerializeField] float damage = 10.0f;


    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = wanderSpeed;
        player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(AIThink());
    }

    IEnumerator AIThink() 
    {
        while (true) 
        {
            // Time before updating status
            thinkDuration = Random.Range(0.5f, 2.0f);

            LookForPlayer();
            SetTarget();
            SetAgentSpeed();
            ProcessBehaviourIfPlayerInChargeArea();

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
            isPlayerLocated = true;
            CheckIfPlayerInChargeArea();
        }
        else
        {
            isPlayerLocated = false;
            isPlayerInChargeArea = false;
        }
    }

    private void SetTarget()
    {
        if (isPlayerLocated)
        {
            ChasePlayer();
        }
        else
        {
            RandomWander();
        }

        navMeshAgent.SetDestination(target);
    }
    private void SetAgentSpeed()
    {
        if (!isPlayerLocated)
        {
            navMeshAgent.acceleration = wanderAcceleration;
            navMeshAgent.speed = wanderSpeed;
        }
        else if (!isPlayerInChargeArea)
        {
            navMeshAgent.acceleration = attackAcceleration;
            navMeshAgent.speed = attackSpeed;
        }
        else
        {
            Debug.Log("Chaaaaaaaaaaarge!!!");
            navMeshAgent.acceleration = chargeAcceleration;
            navMeshAgent.speed = chargeSpeed;
        }
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
        if(distanceToPlayer < chargeDistance)
        {
            isPlayerInChargeArea = true;
        }
        else
        {
            isPlayerInChargeArea = false;
        }
    }

    private void ChasePlayer()
    {
        target = player.transform.position;
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

    public void TakeDamage(float amount)
    {
        StartCoroutine(EnterRedAlertMode());
    }

    private IEnumerator EnterRedAlertMode()
    {
        isInRedAlertMode = true;
        isPlayerLocated = true;
        yield return new WaitForSeconds(redAlertDuration);

        isInRedAlertMode = false;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.transform.parent)
        {
            if (other.transform.parent.CompareTag("Player"))
            {
                other.transform.parent.SendMessage("TakeDamage", damage);
            }
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
