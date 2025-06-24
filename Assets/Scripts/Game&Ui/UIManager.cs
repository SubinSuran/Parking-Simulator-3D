using TMPro;
using UnityEngine;
using UnityEngine.UI; // Required for Image component

public class UIManager : MonoBehaviour
{
    [Header("Game State Panels")]
    public GameObject levelCompleteScreen;
    public GameObject gameCompleteScreen;
    public GameObject gameOverPanel;
    public GameObject pauseMenuPanel;
    public GameObject HUDPanel;
    [Header("Star Display")]
    // Drag your 3 star Image objects from the LevelCompleteScreen panel here
    public RawImage[] levelCompleteStars;

    [Header("In-Game HUD")]
    public TextMeshProUGUI parkIndicatorText;

    void Start()
    {
        HUDPanel.SetActive(true);
        // Make sure all panels are hidden when the game starts
        HideAllScreens();
    }
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

    // --- ADD THESE TWO NEW HANDLER FUNCTIONS ---
    void HandleReadyToPark()
    {
        if (parkIndicatorText != null)
        {
            parkIndicatorText.text = "Press [P] to Park";
            parkIndicatorText.color = Color.green; // Let's use green for "Go"
        }
    }

    void HandleNotReadyToPark()
    {
        if (parkIndicatorText != null)
        {
            parkIndicatorText.text = "Align Vehicle in Zone & Press [P]";
            parkIndicatorText.color = Color.white;
        }
    }
    /// <summary>
    /// Shows the Level Complete screen and sets the correct number of stars.
    /// </summary>
    /// <param name="starsEarned">The number of stars (1, 2, or 3) to display.</param>
    public void ShowLevelCompleteScreen(int starsEarned)
    {
        if (HUDPanel != null) HUDPanel.SetActive(false);
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
    

    public bool IsAnyPanelActive()
    {
        // This will check if any of your main panels are currently active.
        // We don't include the Pause Menu itself in this check.
        if (levelCompleteScreen.activeSelf || gameCompleteScreen.activeSelf || gameOverPanel.activeSelf)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    // --- Other Panel Control Functions ---
    // Add this new public function to UIManager.cs

    public void ResetHUD()
    {
        // We can just call the existing function that already sets the text back to default.
        HandleNotReadyToPark();
    }
    public void ShowGameCompleteScreen()
    {
        if (HUDPanel != null) HUDPanel.SetActive(false);
        if (gameCompleteScreen != null) gameCompleteScreen.SetActive(true);
    }

    public void ShowGameOverScreen()
    {
        if(HUDPanel!=null) HUDPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void ShowPauseMenu()
    {
        if (HUDPanel != null) HUDPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
    }

    public void HidePauseMenu()
    {
        if (HUDPanel != null) HUDPanel.SetActive(true);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
    }
    public void ShowHUD()
    {
        if (HUDPanel != null) HUDPanel.SetActive(true);
    }
    public void HideAllScreens()
    {
        if (levelCompleteScreen != null) levelCompleteScreen.SetActive(false);
        if (gameCompleteScreen != null) gameCompleteScreen.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        
    }
}