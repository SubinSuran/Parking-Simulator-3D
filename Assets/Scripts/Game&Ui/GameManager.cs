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

    // --- Private State Variables ---
    private int currentLevelIndex = 0;
    private GameObject currentLevelInstance;
    private bool isPaused = false;
    private float levelStartTime;
    private bool hasCrashedThisAttempt = false;

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
        StartCoroutine(HandleParkedSuccessRoutine(wasParkedInReverse));
    }

    void OnPlayerCrashed()
    {
        hasCrashedThisAttempt = true;
        isPaused = true;
        SceneFader.instance.FadeTransition(() => {
            Time.timeScale = 0f;
            uiManager.ShowGameOverScreen();
        });
    }

    IEnumerator HandleParkedSuccessRoutine(bool wasParkedInReverse)
    {
        yield return new WaitForSeconds(1f); // Brief pause after parking

        LevelData currentLevelData = levels[currentLevelIndex];

        // --- CHECK FOR FAILURE FIRST ---
        if (currentLevelData.failOnCollision && hasCrashedThisAttempt)
        {
            SceneFader.instance.FadeTransition(() => {
                uiManager.ShowGameOverScreen();
            });
            yield break; // Stop the coroutine
        }
        if (currentLevelData.objective == LevelObjectiveType.ReverseParkOnly && !wasParkedInReverse)
        {
            SceneFader.instance.FadeTransition(() => {
                uiManager.ShowGameOverScreen();
            });
            yield break; // Stop the coroutine
        }

        // --- IF NO FAILURE, CALCULATE STARS ---
        float timeTaken = Time.time - levelStartTime;
        int starsEarned = 1;
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

    // All other functions call the fader...
    public void RestartLevel() { SceneFader.instance.FadeTransition(() => { uiManager.HideAllScreens(); LoadLevel(currentLevelIndex); }); }
    public void LoadNextLevel() { SceneFader.instance.FadeTransition(() => { uiManager.HideAllScreens(); currentLevelIndex++; if (currentLevelIndex < levels.Length) LoadLevel(currentLevelIndex); else uiManager.ShowGameCompleteScreen(); }); }
    public void GoToMainMenu() { Time.timeScale = 1f; SceneFader.instance.FadeToScene("MainMenu"); }

    // --- Core Level Loading ---
    void LoadLevel(int levelIndex)
    {
        uiManager.ResetHUD();
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
        currentLevelInstance = Instantiate(levels[levelIndex].levelPrefab, Vector3.zero, Quaternion.identity);

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