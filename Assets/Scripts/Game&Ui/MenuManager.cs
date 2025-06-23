// File: MenuManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject levelsPanel;

    [Header("Level Selection")]
    public GameObject levelButtonPrefab; // The button prefab we will create
    public Transform levelButtonGrid;   // The parent object to hold the grid of buttons
    public int totalLevelCount = 10;    // The total number of levels in our game

    void Start()
    {
        PopulateLevelButtons();
        // Start on the main menu panel when the scene loads
        OnBackButtonClicked();
    }

    void PopulateLevelButtons()
    {
        // Clear any old buttons in case we call this again
        foreach (Transform child in levelButtonGrid)
        {
            Destroy(child.gameObject);
        }

        int highestLevelUnlocked = SaveManager.LoadHighestLevelUnlocked();

        for (int i = 0; i < totalLevelCount; i++)
        {
            GameObject buttonGO = Instantiate(levelButtonPrefab, levelButtonGrid);
            LevelButton levelButton = buttonGO.GetComponent<LevelButton>();

            levelButton.Setup(i, this);

            // Lock or unlock the button based on saved progress
            if ((i + 1) > highestLevelUnlocked)
            {
                levelButton.SetLocked(true);
            }
            else
            {
                levelButton.SetLocked(false);
            }
        }
    }

    // This is called by the LevelButton when it's clicked
    public void OnLevelSelected(int levelIndex)
    {
        // Save the chosen level so the GameManager can load it
        PlayerPrefs.SetInt("SelectedLevelIndex", levelIndex);
        PlayerPrefs.Save();

        SceneManager.LoadScene("MainGame");
    }

    // --- Functions for the main menu navigation buttons ---
    public void OnLevelsButtonClicked() { mainMenuPanel.SetActive(false); levelsPanel.SetActive(true); }
    public void OnBackButtonClicked() { levelsPanel.SetActive(false); mainMenuPanel.SetActive(true); }
    public void OnExitButtonClicked() { Application.Quit(); }
}