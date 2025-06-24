using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

[System.Serializable]
public class LevelButtonUI
{
    public Button button;
    public TextMeshProUGUI levelText; // Let's add the text here for a complete setup
    public Image[] stars; // Array for the 3 star images on each button
}

public class MenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public CanvasGroup logoCanvasGroup;
    public CanvasGroup tutorialCanvasGroup;
    public GameObject mainMenuPanel;
    public GameObject levelsPanel;

    [Header("Animation")]
    public Animator levelsPanelAnimator;
    public float panelAnimationDuration = 0.5f;

    [Header("Settings")]
    public float introFadeDuration = 1.0f;
    public float logoDisplayDuration = 2.0f;

    [Header("Manual Button Setup")]
    public LevelButtonUI[] levelButtons;

    // Static flag to ensure intro only plays once per game session
    public static bool hasSeenIntro = false;
    private bool tutorialFinished = false;

    void Start()
    {
        // Check if we need to play the intro
        if (hasSeenIntro)
        {
            // If we've already seen it, just show the main menu immediately
            mainMenuPanel.SetActive(true);
            levelsPanel.SetActive(false);
            logoCanvasGroup.gameObject.SetActive(false);
            tutorialCanvasGroup.gameObject.SetActive(false);
            UpdateLevelButtons();
        }
        else
        {
            // If this is the first time, play the intro sequence
            StartCoroutine(StartupSequenceRoutine());
        }
    }

    private IEnumerator StartupSequenceRoutine()
    {
        // Start with all main panels hidden
        mainMenuPanel.SetActive(false);
        levelsPanel.SetActive(false);
        tutorialCanvasGroup.gameObject.SetActive(true);
        tutorialCanvasGroup.alpha = 0;

        // Fade in logo
        logoCanvasGroup.gameObject.SetActive(true);
        yield return FadeCanvasGroup(logoCanvasGroup, 0f, 1f, introFadeDuration);
        yield return new WaitForSeconds(logoDisplayDuration);

        // Cross-fade to tutorial
        StartCoroutine(FadeCanvasGroup(logoCanvasGroup, 1f, 0f, introFadeDuration));
        yield return FadeCanvasGroup(tutorialCanvasGroup, 0f, 1f, introFadeDuration);
        logoCanvasGroup.gameObject.SetActive(false);

        // Wait for player to continue from tutorial
        yield return new WaitUntil(() => tutorialFinished);
        yield return FadeCanvasGroup(tutorialCanvasGroup, 1f, 0f, introFadeDuration);
        tutorialCanvasGroup.gameObject.SetActive(false);

        // Now show the main menu and update the buttons
        mainMenuPanel.SetActive(true);
        UpdateLevelButtons();
        hasSeenIntro = true;
    }

    void UpdateLevelButtons()
    {
        int highestLevelUnlocked = SaveManager.LoadHighestLevelUnlocked();

        for (int i = 0; i < levelButtons.Length; i++)
        {
            // Set level number text
            if (levelButtons[i].levelText != null)
            {
                levelButtons[i].levelText.text = (i + 1).ToString();
            }

            // Lock/Unlock button
            if ((i + 1) > highestLevelUnlocked)
            {
                levelButtons[i].button.interactable = false;
                
            }
            else
            {
                levelButtons[i].button.interactable = true;
                
            }

            // Update star display (we'll add this next)
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        cg.alpha = endAlpha;
    }

    // --- All public functions for buttons remain the same ---
    #region Button Clicks
    public void OnTutorialContinueClicked() { tutorialFinished = true; }
    public void OnLevelSelected(int levelIndex) { SceneFader.instance.FadeToScene("MainGame"); PlayerPrefs.SetInt("SelectedLevelIndex", levelIndex); PlayerPrefs.Save(); }
    public void OnExitButtonClicked() { Application.Quit(); }
    public void OnLevelsButtonClicked() { mainMenuPanel.SetActive(false); levelsPanel.SetActive(true); levelsPanelAnimator.SetBool("IsShown", true); }
    public void OnBackButtonClicked() { StartCoroutine(HideLevelsPanelRoutine()); }
    public void OnResetProgressClicked() { SaveManager.ResetAllProgress(); MenuManager.hasSeenIntro = false; SceneManager.LoadScene("MainMenu"); }
    IEnumerator HideLevelsPanelRoutine() { levelsPanelAnimator.SetBool("IsShown", false); yield return new WaitForSeconds(panelAnimationDuration); mainMenuPanel.SetActive(true); levelsPanel.SetActive(false); }
    #endregion
}