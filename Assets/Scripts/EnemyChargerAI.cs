using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChargerAI : MonoBehaviour
{
    [SerializeField] Transform player = null;
    [SerializeField] bool debug = false;

    private LineRenderer lineRenderer = null;


    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.destination = player.position;

        if (debug){
            lineRenderer = gameObject.AddComponent(typeof(LineRenderer)) as LineRenderer;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ShowPathIfDebugMode();
    }

    private void ShowPathIfDebugMode()
    {
        if (!debug || !navMeshAgent.hasPath) {return;}

        lineRenderer.positionCount = navMeshAgent.path.corners.Length;
        lineRenderer.SetPositions(navMeshAgent.path.corners);
        lineRenderer.enabled = true;
    }
}
