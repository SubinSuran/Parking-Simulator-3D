using UnityEngine;
using UnityEngine.UI; // Required for Image component

public class UIManager : MonoBehaviour
{
    [Header("Game State Panels")]
    public GameObject levelCompleteScreen;
    public GameObject gameCompleteScreen;
    public GameObject gameOverPanel;
    public GameObject pauseMenuPanel;

    [Header("Star Display")]
    // Drag your 3 star Image objects from the LevelCompleteScreen panel here
    public RawImage[] levelCompleteStars;

    void Start()
    {
        // Make sure all panels are hidden when the game starts
        HideAllScreens();
    }

    /// <summary>
    /// Shows the Level Complete screen and sets the correct number of stars.
    /// </summary>
    /// <param name="starsEarned">The number of stars (1, 2, or 3) to display.</param>
    public void ShowLevelCompleteScreen(int starsEarned)
    {
        if (levelCompleteScreen != null)
        {
            levelCompleteScreen.SetActive(true);

            // Loop through the star images you've assigned in the Inspector
            for (int i = 0; i < levelCompleteStars.Length; i++)
            {
                // If the current star's index is less than the number of stars earned, show it.
                // Example: If starsEarned is 2, it will enable star 0 and star 1.
                if (i < starsEarned)
                {
                    levelCompleteStars[i].enabled = true; // Makes the star visible
                }
                else
                {
                    levelCompleteStars[i].enabled = false; // Makes the star invisible
                }
            }
        }
    }

    // --- Other Panel Control Functions ---

    public void ShowGameCompleteScreen()
    {
        if (gameCompleteScreen != null) gameCompleteScreen.SetActive(true);
    }

    public void ShowGameOverScreen()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void ShowPauseMenu()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
    }

    public void HidePauseMenu()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
    }

    public void HideAllScreens()
    {
        if (levelCompleteScreen != null) levelCompleteScreen.SetActive(false);
        if (gameCompleteScreen != null) gameCompleteScreen.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
    }
}