using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Game State Panels")]
    public GameObject levelCompleteScreen;
    public GameObject gameCompleteScreen;
    public GameObject gameOverPanel;
    public GameObject pauseMenuPanel;
    public GameObject HUDPanel;

    [Header("Display Elements")]
    // Use Image for sprites. RawImage is for raw texture data.
    public RawImage[] levelCompleteStars;
    public TextMeshProUGUI gameOverMessageText;
    public TextMeshProUGUI parkIndicatorText;

    void Start()
    {
        // When the game starts, show the main HUD and hide all other pop-up panels.
        ResetHUD();
    }

    // Subscribe to events from the ParkingSpace script
    private void OnEnable()
    {
        ParkingSpace.OnReadyToPark += HandleReadyToPark;
        ParkingSpace.OnNotReadyToPark += HandleNotReadyToPark;
    }

    private void OnDisable()
    {
        ParkingSpace.OnReadyToPark -= HandleReadyToPark;
        ParkingSpace.OnNotReadyToPark -= HandleNotReadyToPark;
    }

    // --- Handlers for Parking Indicator ---
    void HandleReadyToPark() { if (parkIndicatorText != null) { parkIndicatorText.text = "Press [P] to Park"; parkIndicatorText.color = Color.green; } }
    void HandleNotReadyToPark() { if (parkIndicatorText != null) { parkIndicatorText.text = "Align Vehicle in Zone"; parkIndicatorText.color = Color.white; } }

    // --- Public Functions called by GameManager ---

    // In UIManager.cs

    public void ShowLevelCompleteScreen(int starsEarned)
    {
        if (HUDPanel != null) HUDPanel.SetActive(false);
        if (levelCompleteScreen != null)
        {
            levelCompleteScreen.SetActive(true);

            // Loop through the star images you've assigned in the Inspector
            for (int i = 0; i < levelCompleteStars.Length; i++)
            {
                // --- NEW SAFETY CHECK ---
                // First, make sure the slot in the array isn't empty
                if (levelCompleteStars[i] != null)
                {
                    // If the current star's index is less than the number of stars earned, show it.
                    levelCompleteStars[i].enabled = (i < starsEarned);
                }
            }
        }
    }

    // This function now correctly takes a string argument
    public void ShowGameOverScreen()
    {
        if (HUDPanel != null) HUDPanel.SetActive(false);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void ShowPauseMenu() { if (HUDPanel != null) HUDPanel.SetActive(false); if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true); }
    public void HidePauseMenu() { if (HUDPanel != null) HUDPanel.SetActive(true); if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false); }
    public void ShowGameCompleteScreen() { if (HUDPanel != null) HUDPanel.SetActive(false); if (gameCompleteScreen != null) gameCompleteScreen.SetActive(true); }

    public void ResetHUD()
    {
        if (HUDPanel != null) HUDPanel.SetActive(true);
        HandleNotReadyToPark();
        HideAllScreens();
    }

    public void HideAllScreens()
    {
        if (levelCompleteScreen != null) levelCompleteScreen.SetActive(false);
        if (gameCompleteScreen != null) gameCompleteScreen.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
    }

    public bool IsAnyPanelActive()
    {
        return levelCompleteScreen.activeSelf || gameCompleteScreen.activeSelf || gameOverPanel.activeSelf;
    }
}