using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Manages all In-Game UI panels and elements, like the HUD, Pause Menu, Win/Lose screens etc.
/// It takes commands from the GameManager and triggers sound effects.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Game State Panels")]
    public GameObject levelCompleteScreen;
    public GameObject gameCompleteScreen;
    public GameObject gameOverPanel;
    public GameObject pauseMenuPanel;
    public GameObject HUDPanel;

    [Header("Display Elements")]
    public Image[] levelCompleteStars;
    public TextMeshProUGUI gameOverMessageText;
    public TextMeshProUGUI parkIndicatorText;
    public ObjectiveText objectiveDisplay;

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
    void HandleReadyToPark()
    {
        if (parkIndicatorText != null)
        {
            parkIndicatorText.text = "Press [P] to Park";
            parkIndicatorText.color = Color.green;
        }
    }
    void HandleNotReadyToPark()
    {
        if (parkIndicatorText != null)
        {
            parkIndicatorText.text = "Align Vehicle in Zone";
            parkIndicatorText.color = Color.white;
        }
    }

    // --- Public Functions called by GameManager ---

    public void ShowLevelCompleteScreen(int starsEarned)
    {
        if (levelCompleteScreen != null)
        {
           
            if (HUDPanel != null) HUDPanel.SetActive(false);
            levelCompleteScreen.SetActive(true);

            for (int i = 0; i < levelCompleteStars.Length; i++)
            {
                if (levelCompleteStars[i] != null)
                {
                    levelCompleteStars[i].enabled = (i < starsEarned);
                }
            }
        }
    }

    public void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
        {
           
            if (HUDPanel != null) HUDPanel.SetActive(false);
            
            gameOverPanel.SetActive(true);
        }
    }

    // --- THIS IS THE NEW FUNCTION ---
    /// <summary>
    /// Sets the text for the level's objective on the HUD.
    /// </summary>
    public void SetObjectiveText(string message)
    {
        if (objectiveDisplay != null)
        {
            objectiveDisplay.SetObjectiveText(message);
        }
    }

    public void ShowPauseMenu()
    {
       
        if (HUDPanel != null) HUDPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
    }

    public void ShowGameCompleteScreen()
    {
        
        if (HUDPanel != null) HUDPanel.SetActive(false);
        if (gameCompleteScreen != null) gameCompleteScreen.SetActive(true);
    }

    public void HidePauseMenu()
    {
        if (HUDPanel != null) HUDPanel.SetActive(true);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
    }

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
