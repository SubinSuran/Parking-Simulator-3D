using System;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public static event Action OnPlayerCrashed;

    private bool hasCrashed = false;
    private void OnCollisionEnter(Collision collision)
    {
        // If we have already crashed in this attempt, do nothing
        if (hasCrashed) return;

        // We check if we've hit something tagged as an "Obstacle"
        if (collision.gameObject.CompareTag("Obstacles"))
        {
            Debug.Log("PLAYER CRASHED!");
            hasCrashed = true; // Set the flag so we don't crash again
            OnPlayerCrashed?.Invoke(); // Fire the event for the GameManager to hear
        }
    }

    // The GameManager will call this function every time a new level loads
    public void ResetCrashState()
    {
        hasCrashed = false;
    }
}
