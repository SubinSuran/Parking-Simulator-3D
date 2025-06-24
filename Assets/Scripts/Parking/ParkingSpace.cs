using UnityEngine;
using System;
using System.Collections.Generic; // Needed for Dictionary

/// <summary>
/// Manages a single parking space. It detects if the player's car is correctly positioned
/// and tells the GameManager when the player attempts to park.
/// </summary>
public class ParkingSpace : MonoBehaviour
{
    // --- STATIC EVENTS ---
    // This event tells the GameManager *if* the car was parked in reverse.
    public static event Action<bool> OnParkedSuccess;
    // These events tell the UIManager to update the HUD text (e.g., "Press P to Park").
    public static event Action OnReadyToPark;
    public static event Action OnNotReadyToPark;

    // --- PRIVATE VARIABLES ---
    private CarController carController;    // A reference to the car's main controller script
    private BoxCollider parkingBounds;      // The collider that defines the parking zone

    private bool isPlayerInZone = false;    // Is any part of the car in the zone?
    private bool isReadyToPark = false;     // Are all wheels correctly positioned inside?
    private bool hasWon = false;            // Has the player already won this level?

    // NEW: To track which wheels are inside and their entry times
    private Dictionary<WheelCollider, float> wheelEntryTimes = new Dictionary<WheelCollider, float>();
    private bool frontWheelsEnteredFirst = false;

    private void Awake()
    {
        parkingBounds = GetComponent<BoxCollider>();
        parkingBounds.isTrigger = true; // Ensure the collider acts as a trigger
    }

    private void OnTriggerEnter(Collider other)
    {
        // When the car enters the zone, get a reference to its CarController script
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            carController = other.GetComponent<CarController>();
            // Initialize or clear wheel entry times when the car first enters the zone
            wheelEntryTimes.Clear();
            frontWheelsEnteredFirst = false; // Reset this flag
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When the car leaves, clear all references and reset the state
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            carController = null;
            wheelEntryTimes.Clear(); // Clear tracking when car leaves
            frontWheelsEnteredFirst = false; // Reset this flag
            if (isReadyToPark)
            {
                isReadyToPark = false;
                OnNotReadyToPark?.Invoke(); // Tell the UI to reset its text
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // If the car isn't in the zone or the level is already won, do nothing.
        if (!isPlayerInZone || hasWon || carController == null) // Added null check for carController
        {
            return;
        }

        // Check if all four wheels of the car are inside our parking bounds
        bool allWheelsIn = AreAllWheelsInside();

        // --- State Change Logic for the HUD ---
        if (allWheelsIn && !isReadyToPark)
        {
            // If all wheels are now inside and we weren't ready before, update the state
            isReadyToPark = true;
            OnReadyToPark?.Invoke(); // Fire event to turn HUD text to "Press [P] to Park"
        }
        else if (!allWheelsIn && isReadyToPark)
        {
            // If the wheels are no longer aligned, reset the state
            isReadyToPark = false;
            OnNotReadyToPark?.Invoke(); // Fire event to reset HUD text
        }

        // --- Track Wheel Entry for Reverse Parking Logic ---
        TrackWheelEntry();

        // --- Final Win Condition Check ---
        // If the car is perfectly aligned and the player presses the 'P' key...
        if (isReadyToPark && Input.GetKeyDown(KeyCode.P))
        {
            // Determine if it was a reverse park based on which wheels entered first
            // This is our new, more accurate check for reverse parking.
            bool isParkedInReverse = !frontWheelsEnteredFirst;

            hasWon = true;
            // Tell the GameManager that we've won, and also tell it IF it was a reverse park.
            OnParkedSuccess?.Invoke(isParkedInReverse);
            Debug.Log($"Parking attempt registered! Reverse Park Status (based on wheel entry): {isParkedInReverse}");
        }
    }

    /// <summary>
    /// Checks if all four of the car's WheelColliders are inside this trigger's bounds.
    /// Also, updates the entry times for each wheel.
    /// </summary>
    private bool AreAllWheelsInside()
    {
        if (carController == null) return false;

        bool allInside = true;

        // Get the world position of each wheel and check if it's inside
        // Store the WheelCollider itself for easier tracking
        WheelCollider[] wheels = new WheelCollider[]
        {
            carController.FLWheel,
            carController.FRWheel,
            carController.RLWheel,
            carController.RRWheel
        };

        foreach (WheelCollider wheel in wheels)
        {
            wheel.GetWorldPose(out Vector3 wheelPos, out _);
            if (!parkingBounds.bounds.Contains(wheelPos))
            {
                allInside = false;
            }
        }
        return allInside;
    }

    /// <summary>
    /// Tracks when each wheel enters the parking zone.
    /// This is crucial for determining if front or rear wheels entered first.
    /// </summary>
    private void TrackWheelEntry()
    {
        if (carController == null) return;

        WheelCollider[] allWheels = new WheelCollider[]
        {
            carController.FLWheel,
            carController.FRWheel,
            carController.RLWheel,
            carController.RRWheel
        };

        foreach (WheelCollider wheel in allWheels)
        {
            wheel.GetWorldPose(out Vector3 wheelPos, out _);
            if (parkingBounds.bounds.Contains(wheelPos))
            {
                // If the wheel just entered (wasn't tracked before), record its entry time
                if (!wheelEntryTimes.ContainsKey(wheel))
                {
                    wheelEntryTimes.Add(wheel, Time.time);
                }
            }
            else
            {
                // If a wheel leaves the zone, remove it from tracking
                if (wheelEntryTimes.ContainsKey(wheel))
                {
                    wheelEntryTimes.Remove(wheel);
                }
            }
        }

        // Determine if front wheels consistently entered before rear wheels
        // This logic will be more robust if checked when the car is *almost* in position,
        // or just before the "ready to park" state.
        if (wheelEntryTimes.Count >= 2) // At least two wheels need to be in for a meaningful comparison
        {
            // Simple check: if both front wheels are in AND entered before *any* rear wheels
            bool flIn = wheelEntryTimes.ContainsKey(carController.FLWheel);
            bool frIn = wheelEntryTimes.ContainsKey(carController.FRWheel);
            bool rlIn = wheelEntryTimes.ContainsKey(carController.RLWheel);
            bool rrIn = wheelEntryTimes.ContainsKey(carController.RRWheel);

            if (flIn && frIn) // Both front wheels are inside
            {
                // Check if they entered before any rear wheels that are currently inside
                bool frontBeforeRear = true;
                if (rlIn && wheelEntryTimes[carController.RLWheel] < wheelEntryTimes[carController.FLWheel] && wheelEntryTimes[carController.RLWheel] < wheelEntryTimes[carController.FRWheel])
                {
                    frontBeforeRear = false;
                }
                if (rrIn && wheelEntryTimes[carController.RRWheel] < wheelEntryTimes[carController.FLWheel] && wheelEntryTimes[carController.RRWheel] < wheelEntryTimes[carController.FRWheel])
                {
                    frontBeforeRear = false;
                }

                frontWheelsEnteredFirst = frontBeforeRear;
            }
            else if (rlIn && rrIn) // Both rear wheels are inside
            {
                // Check if they entered before any front wheels that are currently inside
                bool rearBeforeFront = true;
                if (flIn && wheelEntryTimes[carController.FLWheel] < wheelEntryTimes[carController.RLWheel] && wheelEntryTimes[carController.FLWheel] < wheelEntryTimes[carController.RRWheel])
                {
                    rearBeforeFront = false;
                }
                if (frIn && wheelEntryTimes[carController.FRWheel] < wheelEntryTimes[carController.RLWheel] && wheelEntryTimes[carController.FRWheel] < wheelEntryTimes[carController.RRWheel])
                {
                    rearBeforeFront = false;
                }

                frontWheelsEnteredFirst = !rearBeforeFront; // If rear entered first, then front did NOT enter first
            }
        }
    }
}