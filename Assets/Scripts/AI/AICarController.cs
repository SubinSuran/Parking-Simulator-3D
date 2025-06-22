using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class AICarController : MonoBehaviour
{
    [Header("Pathfinding")]
    [Tooltip("The very first waypoint node the car should drive towards.")]
    public WaypointNode startingNode;

    [Header("Sensors")]
    [Tooltip("How far ahead the car 'looks' for obstacles.")]
    public float sensorLength = 5f;

    [Tooltip("The layers that the sensor will detect as obstacles (e.g., other cars).")]
    public LayerMask obstacleLayers;

    private NavMeshAgent agent;
    private WaypointNode currentNode;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(InitializeAgent());
    }

    IEnumerator InitializeAgent()
    {
        yield return new WaitForEndOfFrame();
        currentNode = startingNode;
        if (currentNode != null)
        {
            agent.SetDestination(currentNode.transform.position);
        }
        else
        {
            Debug.LogError("AI Car has no starting node assigned!", this.gameObject);
        }
    }

    void Update()
    {
        // Always check for obstacles in front of the car.
        CheckForObstacles();

        // If the agent is not stopped by an obstacle and is close to its destination, pick a new one.
        if (!agent.isStopped && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextNode();
        }
    }

    void CheckForObstacles()
    {
        RaycastHit hit;
        // We start the raycast slightly in front and up from the car's base position.
        Vector3 sensorStartPos = transform.position + transform.forward * 1.5f;
        sensorStartPos.y += 0.5f;

        // Shoot the raycast forward from the sensor position.
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength, obstacleLayers))
        {
            // Obstacle detected! Stop the agent.
            agent.isStopped = true;
            // For debugging, draw a red line in the scene view to show the sensor hit something.
            Debug.DrawLine(sensorStartPos, hit.point, Color.red);
        }
        else
        {
            // No obstacle, resume movement.
            agent.isStopped = false;
            // For debugging, draw a green line to show the sensor's range.
            Debug.DrawLine(sensorStartPos, sensorStartPos + transform.forward * sensorLength, Color.green);
        }
    }

    // In AICarController.cs

    void GoToNextNode()
    {
        if (currentNode == null || currentNode.nextWaypoints.Count == 0)
        {
            Debug.Log("AI Car reached a dead end or has a broken path. Stopping.", this.gameObject);
            agent.isStopped = true;
            return;
        }

        // Randomly pick one of the possible next waypoints from the list.
        int randomIndex = Random.Range(0, currentNode.nextWaypoints.Count);
        WaypointNode nextNode = currentNode.nextWaypoints[randomIndex];

        // --- NEW SAFETY CHECK ---
        // Before we assign it, make sure the chosen next node actually exists.
        if (nextNode == null)
        {
            Debug.LogError("Waypoint '" + currentNode.name + "' has a missing/null entry in its Next Waypoints list! Stopping car.", this.gameObject);
            agent.isStopped = true;
            return;
        }

        // Set the agent's new destination.
        currentNode = nextNode;
        agent.SetDestination(currentNode.transform.position);
    }
}