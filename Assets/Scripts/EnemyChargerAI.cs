using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChargerAI : MonoBehaviour
{
    [SerializeField] Transform player = null;
    [SerializeField] bool debug = false;
 
    [Header("Distances")]
    [SerializeField] float detectionDistance = 20.0f;
    [SerializeField] float chargeDistance = 10.0f;
    [SerializeField] float wanderDistance = 10.0f;

    [Header("Speeds")]
    [SerializeField] float chargeSpeed = 10.0f;
    [SerializeField] float regularSpeed = 4.0f;

    [Header("Durations")]
    [SerializeField] float redAlertDuration = 5.0f;


    private LineRenderer lineRenderer = null;

    private NavMeshAgent navMeshAgent;
    private Vector3 target;

    private bool isPlayerLocated = false;
    private bool isInRedAlertMode = false;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = regularSpeed;

        if (debug){
            lineRenderer = gameObject.AddComponent(typeof(LineRenderer)) as LineRenderer;
        }

        StartCoroutine(AIThink());
    }

    IEnumerator AIThink() 
    {
        while (true) 
        {
            LookForPlayer();
            ChasePlayerIfLocated();
            RandomWander();

            navMeshAgent.destination = target;
            yield return new WaitForSeconds(Random.Range(0.5f, 2.0f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        ShowPathIfDebugMode();
    }

    private void RandomWander()
    {
        if (!isPlayerLocated)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * wanderDistance;
            randomDirection += transform.position;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randomDirection, out navHit, wanderDistance, NavMesh.AllAreas);
            target = navHit.position;
        }
    }

    private void LookForPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!isInRedAlertMode)
        {
            if (distanceToPlayer < detectionDistance)
            {
                isPlayerLocated = true;
            }
            else
            {
                isPlayerLocated = false;
            }
        }

        if (distanceToPlayer < chargeDistance)
        {
            navMeshAgent.speed = chargeSpeed;
        }
        else
        {
            navMeshAgent.speed = regularSpeed;
        }
    }

    private void ChasePlayerIfLocated()
    {
        if (!isPlayerLocated) { return; }

        target = player.position;
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
