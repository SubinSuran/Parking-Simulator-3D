using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject levelCompleteScreen;
    public GameObject gameCompleteScreen; // For when all levels are beaten

    void Start()
    {
        // Make sure screens are hidden when the game starts
        HideAllScreens();
    }

    public void ShowLevelCompleteScreen()
    {
        if (levelCompleteScreen != null)
        {
            levelCompleteScreen.SetActive(true);
        }
    }

    public void ShowGameCompleteScreen()
    {
        if (gameCompleteScreen != null)
        {
            gameCompleteScreen.SetActive(true);
        }
    }

    public void HideAllScreens()
    {
        if (levelCompleteScreen != null) levelCompleteScreen.SetActive(false);
        if (gameCompleteScreen != null) gameCompleteScreen.SetActive(false);
    }

    // Add this function to your UIManager.cs script

    public void HideLevelCompleteScreen()
    {
        if (levelCompleteScreen != null)
        {
            levelCompleteScreen.SetActive(false);
        }
    }
}