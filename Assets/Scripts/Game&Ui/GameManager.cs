using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Level Management")]
    public LevelData[] levels;
    public Transform playerCar;

    [Header("System References")]
    public UIManager uiManager;
    public ObjectiveText objectiveDisplay;

    // --- Private State Variables ---
    private int currentLevelIndex = 0;
    private GameObject currentLevelInstance;
    private bool isPaused = false;
    private float levelStartTime;
    private bool hasCrashedThisAttempt = false;
    private bool levelEnded = false; // --- NEW --- Prevents win/loss from being called multiple times

    // --- Unity Lifecycle & Event Subscription ---
    void Start()
    {
        currentLevelIndex = PlayerPrefs.GetInt("SelectedLevelIndex", 0);
        LoadLevel(currentLevelIndex);
    }

    private void OnEnable()
    {
        ParkingSpace.OnParkedSuccess += HandleParkedSuccess;
        CollisionHandler.OnPlayerCrashed += OnPlayerCrashed;
    }

    private void OnDisable()
    {
        ParkingSpace.OnParkedSuccess -= HandleParkedSuccess;
        CollisionHandler.OnPlayerCrashed -= OnPlayerCrashed;
    }

    void Update()
    {
        // --- NEW TIME-OUT CHECK ---
        // If the level hasn't already ended...
        if (!levelEnded)
        {
            LevelData currentLevelData = levels[currentLevelIndex];
            // ...check if the time taken has exceeded the absolute game over limit.
            if (Time.time - levelStartTime > currentLevelData.timeForGameOver)
            {
                levelEnded = true; // Mark level as ended
                uiManager.ShowGameOverScreen();
            }
        }

        // Listen for the Escape key to pause the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!uiManager.IsAnyPanelActive() && !isPaused)
            {
                PauseGame();
            }
        }
    }

    // --- Event Handlers ---

    void HandleParkedSuccess(bool wasParkedInReverse)
    {
        // If the level has already ended (e.g., by timeout), do nothing.
        if (levelEnded) return;

        levelEnded = true; // Mark level as ended so the Update timer stops checking
        StartCoroutine(HandleParkedSuccessRoutine(wasParkedInReverse));
    }

    void OnPlayerCrashed()
    {
        hasCrashedThisAttempt = true;
        SceneFader.instance.FadeTransition(() => {
            uiManager.ShowGameOverScreen();
            Time.timeScale = 0f;
        });

    }

    IEnumerator HandleParkedSuccessRoutine(bool wasParkedInReverse)
    {
        yield return new WaitForSeconds(1f); // Brief pause after parking

        LevelData currentLevelData = levels[currentLevelIndex];
        float timeTaken = Time.time - levelStartTime;

        // --- CHECK FOR FAILURE CONDITIONS FIRST ---
        if (currentLevelData.failOnCollision && hasCrashedThisAttempt)
        {
            SceneFader.instance.FadeTransition(() => {
                uiManager.ShowGameOverScreen();
            });
            yield break;
        }
        if (currentLevelData.objective == LevelObjectiveType.ReverseParkOnly && !wasParkedInReverse)
        {
            SceneFader.instance.FadeTransition(() => {
                uiManager.ShowGameOverScreen();
            });
            yield break;
        }
        // --- NEW CHECK FOR 1-STAR TIME LIMIT ---
        if (timeTaken > currentLevelData.timeFor1Star)
        {
            SceneFader.instance.FadeTransition(() => {
                uiManager.ShowGameOverScreen();
            });
            yield break;
        }

        // --- IF NO FAILURE, CALCULATE STARS ---
        int starsEarned = 1; // Default to 1 star if they beat the 1-star time
        if (timeTaken <= currentLevelData.timeFor3Stars) { starsEarned = 3; }
        else if (timeTaken <= currentLevelData.timeFor2Stars) { starsEarned = 2; }

        SaveManager.LevelCompleted(currentLevelIndex);
        SaveManager.SaveStars(currentLevelIndex, starsEarned);

        SceneFader.instance.FadeTransition(() => {
            uiManager.ShowLevelCompleteScreen(starsEarned);
        });
    }

    // --- Pause & UI Button Logic ---
    public void PauseGame()
    {
        isPaused = true;
        SceneFader.instance.FadeTransition(() => {
            Time.timeScale = 0f;
            uiManager.ShowPauseMenu();
        });
    }

    public void ResumeGame()
    {
        isPaused = false;
        SceneFader.instance.FadeTransition(() => {
            uiManager.HidePauseMenu();
            Time.timeScale = 1f;
        });
    }

    public void RestartLevel() { SceneFader.instance.FadeTransition(() => { uiManager.HideAllScreens(); LoadLevel(currentLevelIndex); }); }
    public void LoadNextLevel() { SceneFader.instance.FadeTransition(() => { uiManager.HideAllScreens(); currentLevelIndex++; if (currentLevelIndex < levels.Length) LoadLevel(currentLevelIndex); else uiManager.ShowGameCompleteScreen(); }); }
    public void GoToMainMenu() { Time.timeScale = 1f; SceneFader.instance.FadeToScene("MainMenu"); }

    // --- Core Level Loading ---
    void LoadLevel(int levelIndex)
    {
        // --- NEW --- Reset the level ended flag for the new attempt
        levelEnded = false;

        if (uiManager != null) uiManager.ResetHUD();
        Time.timeScale = 1f;
        isPaused = false;
        hasCrashedThisAttempt = false;
        levelStartTime = Time.time;

        if (playerCar != null)
        {
            CollisionHandler carCollision = playerCar.GetComponent<CollisionHandler>();
            if (carCollision != null) carCollision.ResetCrashState();
        }

        if (currentLevelInstance != null) Destroy(currentLevelInstance);

        LevelData currentLevelData = levels[levelIndex];
        currentLevelInstance = Instantiate(currentLevelData.levelPrefab, Vector3.zero, Quaternion.identity);

        if (objectiveDisplay != null)
        {
            string objectiveMessage = "";
            switch (currentLevelData.objective)
            {
                case LevelObjectiveType.ParkInTime:
                    objectiveMessage = "Objective: Park successfully.";
                    break;
                case LevelObjectiveType.ReverseParkOnly:
                    objectiveMessage = "Objective: Park in REVERSE.";
                    break;
            }
            objectiveDisplay.SetObjectiveText(objectiveMessage);
        }

        Transform startPoint = currentLevelInstance.transform.Find("PlayerStartPoint");
        if (startPoint != null)
        {
            playerCar.position = startPoint.position;
            playerCar.rotation = startPoint.rotation;
            Rigidbody carRb = playerCar.GetComponent<Rigidbody>();
            if (carRb != null)
            {
                carRb.linearVelocity = Vector3.zero;
                carRb.angularVelocity = Vector3.zero;
            }
        }
    }
}
