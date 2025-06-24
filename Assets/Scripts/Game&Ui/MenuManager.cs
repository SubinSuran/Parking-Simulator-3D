using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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
    public Animator levelsPanelAnimator;
    public float panelAnimationDuration = 3f;

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
    // In MenuManager.cs
    public void OnLevelSelected(int levelIndex)
    {
        PlayerPrefs.SetInt("SelectedLevelIndex", levelIndex);
        PlayerPrefs.Save();

        // OLD WAY: SceneManager.LoadScene("MainGame");
        // NEW WAY:
        SceneFader.instance.FadeToScene("MainGame");
    }

    // --- Functions for the main navigation buttons ---
    // In MenuManager.cs

    public void OnLevelsButtonClicked()
    {
         mainMenuPanel.SetActive(false); // We can still do this instantly
        levelsPanel.SetActive(true);
        levelsPanelAnimator.SetBool("IsShown", true);
    }

    public void OnBackButtonClicked()
    {
        // Instead of activating the main menu panel directly, we start the coroutine.
        StartCoroutine(ShowMainMenuAfterAnimation());
    }

    // --- This is the NEW Coroutine ---
    IEnumerator ShowMainMenuAfterAnimation()
    {
        // 1. Tell the levels panel to play its "slide out" animation.
        levelsPanelAnimator.SetBool("IsShown", false);

        // 2. Wait for the duration of the animation.
        yield return new WaitForSeconds(panelAnimationDuration);

        // 3. AFTER the wait is over, activate the main menu panel.
        mainMenuPanel.SetActive(true);
        // And ensure the levels panel is fully disabled.
        levelsPanel.SetActive(false);
    }

    public void OnExitButtonClicked() { Application.Quit(); }
}