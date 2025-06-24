using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// We add this small helper class to make the Inspector setup cleaner.
// This groups a button with its corresponding lock icon.
[System.Serializable]
public class LevelButtonUI
{
    public Button button;
    public GameObject lockIcon;
}

public class MenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject levelsPanel;

    [Header("Manual Button Setup")]
    // Create an array to hold your 10 manual buttons and their lock icons.
    public LevelButtonUI[] levelButtons;

    void Start()
    {
        // When the menu starts, update the lock status of all buttons.
        UpdateLevelButtons();
        // Go to the main menu by default.
        OnBackButtonClicked();
    }

    void UpdateLevelButtons()
    {
        int highestLevelUnlocked = SaveManager.LoadHighestLevelUnlocked();

        // Loop through all the buttons you assigned in the Inspector.
        for (int i = 0; i < levelButtons.Length; i++)
        {
            // Check if this level should be locked.
            if ((i + 1) > highestLevelUnlocked)
            {
                // Lock the button
                levelButtons[i].button.interactable = false;
                if (levelButtons[i].lockIcon != null)
                {
                    levelButtons[i].lockIcon.SetActive(true);
                }
            }
            else
            {
                // Unlock the button
                levelButtons[i].button.interactable = true;
                if (levelButtons[i].lockIcon != null)
                {
                    levelButtons[i].lockIcon.SetActive(false);
                }
            }
        }
    }

    // This single function will be called by ALL level buttons.
    // We will tell each button which levelIndex to send.
    public void OnLevelSelected(int levelIndex)
    {
        Debug.Log("Level button " + (levelIndex + 1) + " was clicked.");
        PlayerPrefs.SetInt("SelectedLevelIndex", levelIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainGame");
    }

    // --- Functions for the main navigation buttons ---
    public void OnLevelsButtonClicked() { mainMenuPanel.SetActive(false); levelsPanel.SetActive(true); }
    public void OnBackButtonClicked() { levelsPanel.SetActive(false); mainMenuPanel.SetActive(true); }
    public void OnExitButtonClicked() { Application.Quit(); }
}