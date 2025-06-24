using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AICarController : MonoBehaviour
{
    [Header("Pathfinding")]
    [Tooltip("The very first waypoint node the car should drive towards.")]
    public WaypointNode startingNode;

    // --- MODIFIED FOR SENSORS ---
    [Header("Sensors")]
    [Tooltip("How far ahead the car 'looks' for obstacles.")]
    public float sensorLength = 5f;
    [Tooltip("The layers that the sensor will detect as obstacles (e.g., other cars).")]
    public LayerMask obstacleLayers;
    [Tooltip("How far to the side (left/right) the side sensors are positioned.")]
    public float sideSensorOffset = 0.75f; // Adjust this value based on your car's width
    [Tooltip("Offset forward from the car's pivot for the sensor origin.")]
    public float sensorForwardOffset = 1.5f;
    [Tooltip("Vertical offset for the sensors to be slightly above ground.")]
    public float sensorHeightOffset = 0.5f;
    // --- END OF MODIFIED VARIABLES ---

    private NavMeshAgent agent;
    private WaypointNode currentNode;

    private float originalSpeed; // To remember the car's top speed


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentNode = startingNode; // This will now be assigned by the TrafficSpawner

        originalSpeed = agent.speed;

        if (currentNode != null)
        {
            agent.SetDestination(currentNode.transform.position);
        }
        else
        {
            // This error will now only happen if the spawner fails for some reason.
            Debug.LogError("AI Car has no starting node assigned by its spawner!", this.gameObject);
        }
    }

    void Update()
    {
        CheckForObstacles(); // Call the sensor logic every frame

        // If the agent is close to its destination, it's time to pick a new one.
        // Only proceed if not stopped by an obstacle
        if (!agent.isStopped && agent.pathPending == false && agent.remainingDistance < 0.5f)
        {
            GoToNextNode();
        }
    }

    void GoToNextNode()
    {
        // Check if the current node has any connections
        if (currentNode == null || currentNode.nextWaypoints.Count == 0)
        {
            // If it's a dead end, we can just destroy the car or stop it.
            Debug.Log("AI Car reached a dead end or has a null current node. Stopping.");
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

    /// <summary>
    /// Checks for obstacles using three forward-facing raycasts (center, left, right).
    /// Stops the agent if any obstacle is detected.
    /// </summary>
    void CheckForObstacles()
    {
        RaycastHit hit;
        bool obstacleDetected = false;

        // Calculate the base starting position for all sensors (front-center of the car)
        Vector3 baseSensorStartPos = transform.position + transform.forward * sensorForwardOffset;
        baseSensorStartPos.y += sensorHeightOffset; // Lift sensors slightly off the ground

        // --- CENTER SENSOR ---
        Vector3 centerSensorPos = baseSensorStartPos;
        if (Physics.Raycast(centerSensorPos, transform.forward, out hit, sensorLength, obstacleLayers))
        {
            obstacleDetected = true;
            Debug.DrawLine(centerSensorPos, hit.point, Color.red);
        }
        else
        {
            Debug.DrawLine(centerSensorPos, centerSensorPos + transform.forward * sensorLength, Color.green);
        }

        // --- RIGHT SENSOR ---
        Vector3 rightSensorPos = baseSensorStartPos + transform.right * sideSensorOffset;
        if (Physics.Raycast(rightSensorPos, transform.forward, out hit, sensorLength, obstacleLayers))
        {
            obstacleDetected = true;
            Debug.DrawLine(rightSensorPos, hit.point, Color.red);
        }
        else
        {
            Debug.DrawLine(rightSensorPos, rightSensorPos + transform.forward * sensorLength, Color.green);
        }

        // --- LEFT SENSOR ---
        Vector3 leftSensorPos = baseSensorStartPos - transform.right * sideSensorOffset;
        if (Physics.Raycast(leftSensorPos, transform.forward, out hit, sensorLength, obstacleLayers))
        {
            obstacleDetected = true;
            Debug.DrawLine(leftSensorPos, hit.point, Color.red);
        }
        else
        {
            Debug.DrawLine(leftSensorPos, leftSensorPos + transform.forward * sensorLength, Color.green);
        }

        // --- Apply movement state based on obstacle detection ---
        if (obstacleDetected)
        {
            agent.isStopped = true; // Stop if any sensor detects an obstacle
            agent.speed = 0f; // Ensure speed is also set to zero
        }
        else
        {
            // Only resume if not already stopped by pathing (e.g., dead end)
            if (agent.isStopped)
            {
                // If it was stopped due to an obstacle, re-enable movement
                agent.isStopped = false;
                agent.speed = originalSpeed; // Restore original speed
            }
            // If agent wasn't stopped by an obstacle, ensure speed is normal
            agent.speed = originalSpeed;
        }
    }
}