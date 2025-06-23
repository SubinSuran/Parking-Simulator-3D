using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AICarController : MonoBehaviour
{
    [Header("Pathfinding")]
    [Tooltip("The very first waypoint node the car should drive towards.")]
    public WaypointNode startingNode;

    // --- ADDED FOR SENSORS ---
    [Header("Sensors")]
    [Tooltip("How far ahead the car 'looks' for obstacles.")]
    public float sensorLength = 5f;
    [Tooltip("The layers that the sensor will detect as obstacles (e.g., other cars).")]
    public LayerMask obstacleLayers;
    // --- END OF ADDED VARIABLES ---

    private NavMeshAgent agent;
    private WaypointNode currentNode;

    // --- ADDED FOR SENSORS ---
    private float originalSpeed; // To remember the car's top speed
    // --- END OF ADDED VARIABLES ---


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentNode = startingNode;

        // --- ADDED FOR SENSORS ---
        originalSpeed = agent.speed; // Store the original speed from the Inspector
        // --- END OF ADDED LOGIC ---

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
        // --- ADDED FOR SENSORS ---
        CheckForObstacles(); // Call the sensor logic every frame
        // --- END OF ADDED LOGIC ---

        // If the agent is close to its destination, it's time to pick a new one.
        if (agent.pathPending == false && agent.remainingDistance < 0.5f)
        {
            GoToNextNode();
        }
    }

    void GoToNextNode()
    {
        // Check if the current node has any connections
        if (currentNode.nextWaypoints.Count == 0)
        {
            // If it's a dead end, we can just destroy the car or stop it.
            Debug.Log("AI Car reached a dead end.");
            agent.isStopped = true;
            return;
        }

        // Randomly pick one of the possible next waypoints from the list.
        int randomIndex = Random.Range(0, currentNode.nextWaypoints.Count);
        currentNode = currentNode.nextWaypoints[randomIndex];

        // Set the agent's new destination.
        // Safety check for missing nodes in the list
        if (currentNode != null)
        {
            agent.SetDestination(currentNode.transform.position);
        }
        else
        {
            Debug.LogError("A waypoint has a missing/null entry in its Next Waypoints list! Stopping car.", this.gameObject);
            agent.isStopped = true;
        }
    }

    // --- PASTED CheckForObstacles METHOD ---
    // In AICarController.cs

    void CheckForObstacles()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position + transform.forward * 1.5f;
        sensorStartPos.y += 0.5f;

        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength, obstacleLayers))
        {
            // --- CHANGED LOGIC ---
            // Obstacle detected! Stop the agent completely.
            agent.isStopped = true;

            Debug.DrawLine(sensorStartPos, hit.point, Color.red);
        }
        else
        {
            // --- CHANGED LOGIC ---
            // No obstacle, tell the agent it's clear to move again.
            agent.isStopped = false;

            // We also make sure the speed is at its normal value.
            agent.speed = originalSpeed;

            Debug.DrawLine(sensorStartPos, sensorStartPos + transform.forward * sensorLength, Color.green);
        }
    }
    // --- END OF PASTED METHOD ---
}