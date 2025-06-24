using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ParkingRouteGuide : MonoBehaviour
{
    [SerializeField] private Transform parkingSpot;        // Assign your existing parking spot in the Inspector
    [SerializeField] private Transform player;             // Assign the player's car
    [SerializeField] private LineRenderer pathLine;        // Assign the LineRenderer
    [SerializeField] private float pathHeightOffset = 1.25f;
    [SerializeField] private float pathUpdateInterval = 0.25f;

    private Coroutine drawPathRoutine;

    private void Start()
    {
        // Find the GameObject named "car" and then get its Transform component
        GameObject playerGameObject = GameObject.Find("Car");

        if (playerGameObject != null)
        {
            // Assign the Transform component to your 'player' variable
            player = playerGameObject.transform;
        }
        else
        {
            Debug.LogError("Player GameObject named 'car' not found in the hierarchy! The parking route guide will not function.", this);
            // Optionally disable the script if the player isn't found
            enabled = false;
            return;
        }
        if (drawPathRoutine != null)
            StopCoroutine(drawPathRoutine);

        drawPathRoutine = StartCoroutine(DrawPathToSpot());
    }

    private IEnumerator DrawPathToSpot()
    {
        WaitForSeconds wait = new WaitForSeconds(pathUpdateInterval);
        NavMeshPath path = new NavMeshPath();

        while (parkingSpot != null && player != null)
        {
            if (NavMesh.CalculatePath(player.position, parkingSpot.position, NavMesh.AllAreas, path))
            {
                pathLine.positionCount = path.corners.Length;
                for (int i = 0; i < path.corners.Length; i++)
                    pathLine.SetPosition(i, path.corners[i] + Vector3.up * pathHeightOffset);
            }
            else
            {
                Debug.LogWarning("Path could not be calculated.");
                pathLine.positionCount = 0;
            }

            yield return wait;
        }

        pathLine.positionCount = 0; // clear path if parking spot is destroyed or missing
    }
}
