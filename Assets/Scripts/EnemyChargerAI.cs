using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChargerAI : MonoBehaviour
{
    [SerializeField] Transform player = null;
    [SerializeField] bool debug = false;
    [SerializeField] float viewCone = 60.0f;
 
    [Header("Distances")]
    [SerializeField] float detectionDistance = 20.0f;
    [SerializeField] float chargeDistance = 10.0f;
    [SerializeField] float wanderDistance = 10.0f;

    [Header("Speeds")]
    [SerializeField] float wanderSpeed = 5.0f;
    [SerializeField] float attackSpeed = 10.0f;
    [SerializeField] float chargeSpeed = 50.0f;

    [Header("Durations")]
    [SerializeField] float redAlertDuration = 5.0f;

    private LineRenderer lineRenderer = null;

    private NavMeshAgent navMeshAgent;

    private bool isPlayerInDetectionArea = false;
    private bool isPlayerVisible = false;

    private bool isPlayerLocated = false;
    private bool isInRedAlertMode = false;
    private float distanceToPlayer;
    private float angleToPlayer;
    private RaycastHit rayToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = wanderSpeed;
        lineRenderer = GetComponent<LineRenderer>();

        StartCoroutine(AIThink());
    }

    // Update is called once per frame
    void Update()
    {
        ShowPathIfDebugMode();
    }

    IEnumerator AIThink() 
    {
        while (true) 
        {
            LookForPlayer();
            SetAgentSpeed();
            ChasePlayerIfLocated();
            RandomWander();

            yield return new WaitForSeconds(Random.Range(0.5f, 2.0f));
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
        }
    }

    private void SetAgentSpeed()
    {
        if (!isPlayerLocated)
        {
            navMeshAgent.speed = wanderSpeed;
        }
        else if (distanceToPlayer > chargeDistance)
        {
            navMeshAgent.speed = attackSpeed;
        }
        else
        {
            navMeshAgent.speed = chargeSpeed;
        }
    }
    private void ChasePlayerIfLocated()
    {
        if (!isPlayerLocated) { return; }
        navMeshAgent.SetDestination(player.position);
    }

    private void RandomWander()
    {
        if (!isPlayerLocated)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * wanderDistance;
            randomDirection += transform.position;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randomDirection, out navHit, wanderDistance, NavMesh.AllAreas);
            navMeshAgent.SetDestination(navHit.position);
        }
    }

    private void CheckIfPlayerIsVisible()
    {
        if (!isPlayerInDetectionArea) { return; }

        if (Physics.Linecast(transform.position, player.position, out rayToPlayer))
        {
            Transform hitLocation = rayToPlayer.collider.gameObject.transform;            
            if(hitLocation.parent)
            {
                if (hitLocation.parent.CompareTag("Player"))
                {
                    isPlayerVisible = true;
                    return;
                }
            }
        }

        isPlayerVisible = false;
    }

    private void CheckIfPlayerInDetectionArea()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.position);
        angleToPlayer = Vector3.Angle(transform.forward, player.position - transform.position);

        if (distanceToPlayer < detectionDistance && angleToPlayer < viewCone)
        {
            isPlayerInDetectionArea = true;
        }
        else
        {
            isPlayerInDetectionArea = false;
        }
    }

    private void ShowPathIfDebugMode()
    {
        if (!debug || !navMeshAgent.hasPath) { return; }

        lineRenderer.positionCount = navMeshAgent.path.corners.Length;
        lineRenderer.SetPositions(navMeshAgent.path.corners);
        lineRenderer.enabled = true;
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
}
