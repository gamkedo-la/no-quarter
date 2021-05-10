using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChargerAI : MonoBehaviour
{
    [SerializeField] Transform player = null;
    [SerializeField] float detectionDistance = 20.0f;
    [SerializeField] bool debug = false;

    private LineRenderer lineRenderer = null;

    private NavMeshAgent navMeshAgent;

    private bool isPlayerLocated = false;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (debug){
            lineRenderer = gameObject.AddComponent(typeof(LineRenderer)) as LineRenderer;
        }
    }

    // Update is called once per frame
    void Update()
    {
        LookForPlayer();
        ChasePlayerIfLocated();

        ShowPathIfDebugMode();
    }

    private void LookForPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionDistance)
        {
            isPlayerLocated = true;
        }
        else
        {
            isPlayerLocated = false;
        }
    }

    private void ChasePlayerIfLocated()
    {
        if (!isPlayerLocated) { return; }

        navMeshAgent.destination = player.position;
    }

    private void ShowPathIfDebugMode()
    {
        if (!debug || !navMeshAgent.hasPath) { return; }

        lineRenderer.positionCount = navMeshAgent.path.corners.Length;
        lineRenderer.SetPositions(navMeshAgent.path.corners);
        lineRenderer.enabled = true;
    }
}
