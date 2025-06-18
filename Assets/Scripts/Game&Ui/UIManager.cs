// Create a new C# script named "UIManager.cs"

using UnityEngine;
using UnityEngine.UI; // Make sure to include this for UI elements

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject levelCompleteScreen;

    void Start()
    {
        // Make sure the win screen is hidden when the game starts
        if (levelCompleteScreen != null)
        {
            levelCompleteScreen.SetActive(false);
        }
    }

    public void ShowLevelCompleteScreen()
    {
        if (levelCompleteScreen != null)
        {
            levelCompleteScreen.SetActive(true);
        }
    }
}