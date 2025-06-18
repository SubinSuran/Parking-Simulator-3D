// Create a new C# script named "GameManager.cs"

using UnityEngine;

public class GameManager : MonoBehaviour
{
    // A reference to the UIManager to give it commands
    public UIManager uiManager;

    // This function is called when the object becomes enabled and active.
    private void OnEnable()
    {
        // Start listening for the OnParkedSuccess event
        ParkingSpace.OnParkedSuccess += HandleParkedSuccess;
    }

    // This function is called when the object becomes disabled or inactive.
    private void OnDisable()
    {
        // Stop listening to prevent errors
        ParkingSpace.OnParkedSuccess -= HandleParkedSuccess;
    }

    private void HandleParkedSuccess()
    {
        Debug.Log("GAME MANAGER HEARD: Level Complete! Telling UI to update.");

        // Tell the UI Manager to show the win screen
        if (uiManager != null)
        {
            uiManager.ShowLevelCompleteScreen();
        }
    }
}