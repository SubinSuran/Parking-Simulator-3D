// Create a new C# script named "ParkingSpace.cs"

using UnityEngine;
using System; // Required for using the 'Action' type

public class ParkingSpace : MonoBehaviour
{
    [Header("Parking Settings")]
    [Tooltip("How long the car must be stationary inside the trigger to win.")]
    public float timeToPark = 2f;

    [Tooltip("The maximum speed the car can have to be considered 'stopped'.")]
    public float maxSpeedThreshold = 0.1f;

    // This is a professional way to send events. Our GameManager will listen to this.
    public static event Action OnParkedSuccess;

    private float stationaryTimer = 0f;
    private bool isCarInZone = false;
    private bool hasWon = false;
    private Rigidbody carRigidbody;

    // This ensures the object always has a BoxCollider component.
    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered is the player's car
        if (other.CompareTag("Player"))
        {
            isCarInZone = true;
            carRigidbody = other.GetComponent<Rigidbody>();
        }
    }

    // Replace the old OnTriggerStay with this one in ParkingSpace.cs

    private void OnTriggerStay(Collider other)
    {
        // First, check if we have already won. If so, do nothing.
        if (hasWon) return;

        if (isCarInZone && carRigidbody != null)
        {
            if (carRigidbody.linearVelocity.magnitude < maxSpeedThreshold)
            {
                stationaryTimer += Time.deltaTime;

                if (stationaryTimer >= timeToPark)
                {
                    // We have successfully parked!
                    // Set the flag to true IMMEDIATELY to stop this from running again.
                    hasWon = true;

                    Debug.Log("PARKED SUCCESSFULLY!");

                    OnParkedSuccess?.Invoke();
                }
            }
            else
            {
                stationaryTimer = 0f;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // If the car leaves the zone, reset everything
        if (other.CompareTag("Player"))
        {
            isCarInZone = false;
            carRigidbody = null;
            stationaryTimer = 0f;
        }
    }
}