using UnityEngine;
using System;
using TMPro; // We'll add a direct text reference here to simplify things

public class ParkingSpace : MonoBehaviour
{
    // --- STATIC EVENTS ---
    // These will tell the UIManager to change the HUD text color and message
    public static event Action OnReadyToPark;
    public static event Action OnNotReadyToPark;
    // This is the original event to tell the GameManager we have won
    public static event Action OnParkedSuccess;

    // --- PRIVATE VARIABLES ---
    private CarController carController; // A reference to the car's main controller script
    private Transform[] wheelTransforms; // We will get the wheel transforms from the CarController
    private BoxCollider parkingBounds;   // The bounds of our parking zone

    private bool isPlayerInZone = false;
    private bool isReadyToPark = false;
    private bool hasWon = false;

    private void Awake()
    {
        parkingBounds = GetComponent<BoxCollider>();
        parkingBounds.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // When the car enters, get its CarController to access the wheels
            carController = other.GetComponent<CarController>();
            isPlayerInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // When the car leaves, clear the reference and reset the UI
            isPlayerInZone = false;
            carController = null;
            if (isReadyToPark)
            {
                isReadyToPark = false;
                OnNotReadyToPark?.Invoke();
            }
        }
    }

    // We use Update to check the conditions every frame
    private void Update()
    {
        // If the car is not in the zone or we've already won, do nothing.
        if (!isPlayerInZone || hasWon)
        {
            return;
        }

        // Check if all wheels are inside the parking bounds
        bool allWheelsIn = AreAllWheelsInside();

        // --- State Change Logic ---
        if (allWheelsIn && !isReadyToPark)
        {
            // We just became ready to park
            isReadyToPark = true;
            OnReadyToPark?.Invoke(); // Fire event to turn HUD text red
        }
        else if (!allWheelsIn && isReadyToPark)
        {
            // We are no longer ready to park
            isReadyToPark = false;
            OnNotReadyToPark?.Invoke(); // Fire event to reset HUD text
        }

        // --- Final Win Condition ---
        // If we are ready to park and the player presses 'P'
        if (isReadyToPark && Input.GetKeyDown(KeyCode.P))
        {
            hasWon = true;
            OnParkedSuccess?.Invoke(); // Fire event to complete the level
            Debug.Log("PARKED SUCCESSFULLY by pressing P!");
        }
    }

    private bool AreAllWheelsInside()
    {
        if (carController == null) return false;

        // Get the world position of each wheel collider and check if it's inside our parking box
        Vector3 flPos, frPos, rlPos, rrPos;
        carController.FLWheel.GetWorldPose(out flPos, out _);
        carController.FRWheel.GetWorldPose(out frPos, out _);
        carController.RLWheel.GetWorldPose(out rlPos, out _);
        carController.RRWheel.GetWorldPose(out rrPos, out _);

        // Check if all four points are inside the trigger bounds
        return parkingBounds.bounds.Contains(flPos) &&
               parkingBounds.bounds.Contains(frPos) &&
               parkingBounds.bounds.Contains(rlPos) &&
               parkingBounds.bounds.Contains(rrPos);
    }
}