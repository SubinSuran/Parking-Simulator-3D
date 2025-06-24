using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject levelCompleteScreen;
    public GameObject gameCompleteScreen;
    public GameObject gameOverPanel; // --- NEW --- From the collision system
    public GameObject pauseMenuPanel;  // --- NEW --- For the pause menu

    void Start()
    {
        // Make sure all panels are hidden when the game starts
        HideAllScreens();
    }

    // --- NEW: Functions to control the Pause Menu ---
    public void ShowPauseMenu()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
    }

    public void HidePauseMenu()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
    }
    // --- End of new functions ---

    public void ShowLevelCompleteScreen()
    {
        if (levelCompleteScreen != null) levelCompleteScreen.SetActive(true);
    }

    public void ShowGameCompleteScreen()
    {
        if (gameCompleteScreen != null) gameCompleteScreen.SetActive(true);
    }

    public void ShowGameOverScreen()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void HideAllScreens()
    {
        if (levelCompleteScreen != null) levelCompleteScreen.SetActive(false);
        if (gameCompleteScreen != null) gameCompleteScreen.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false); // --- NEW ---
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);   // --- NEW ---
    }
}