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

    void Start()
    {
        uiManager.HUDPanel.SetActive(true);
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

    void HandleParkedSuccess()
    {
        StartCoroutine(HandleParkedSuccessRoutine());
    }

    void OnPlayerCrashed()
    {
        StartCoroutine(HandleGameOverRoutine());
    }

    IEnumerator HandleParkedSuccessRoutine()
    {
        yield return new WaitForSeconds(1f); // Wait 1 second after parking

        float timeTaken = Time.time - levelStartTime;
        int starsEarned = 0;
        LevelData currentLevelData = levels[currentLevelIndex];

        if (timeTaken <= currentLevelData.timeFor3Stars) { starsEarned = 3; }
        else if (timeTaken <= currentLevelData.timeFor2Stars) { starsEarned = 2; }
        else { starsEarned = 1; }

        SaveManager.LevelCompleted(currentLevelIndex);
        SaveManager.SaveStars(currentLevelIndex, starsEarned);

        // Use the fader to show the win screen
        SceneFader.instance.FadeTransition(() => {
            uiManager.ShowLevelCompleteScreen(starsEarned);
        });
    }

    IEnumerator HandleGameOverRoutine()
    {

        yield return new WaitForSeconds(1f); // Wait 3 seconds after crashing

        // Use the fader to show the game over screen
        SceneFader.instance.FadeTransition(() => {
            Time.timeScale = 0f;
            uiManager.ShowGameOverScreen();
        });
    }

    // --- Pause & UI Button Logic ---

    public void PauseGame()
    {
        isPaused = true;
        // Use the fader to show the pause menu
        SceneFader.instance.FadeTransition(() => {
            Time.timeScale = 0f;
            uiManager.ShowPauseMenu();
        });
    }

    public void ResumeGame()
    {
        isPaused = false;
        // Use the fader to hide the pause menu and resume
        SceneFader.instance.FadeTransition(() => {
            uiManager.HidePauseMenu();
            Time.timeScale = 1f;
        });
    }

    public void RestartLevel()
    {
        SceneFader.instance.FadeTransition(() => {
            uiManager.HideAllScreens();
            LoadLevel(currentLevelIndex);
        });
    }

    public void LoadNextLevel()
    {
        SceneFader.instance.FadeTransition(() => {
            uiManager.HideAllScreens();
            currentLevelIndex++;
            if (currentLevelIndex < levels.Length)
            {
                LoadLevel(currentLevelIndex);
            }
            else
            {
                uiManager.ShowGameCompleteScreen();
            }
        });
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneFader.instance.FadeToScene("MainMenu");
    }

    // --- Core Level Loading (No Changes Here) ---
    void LoadLevel(int levelIndex)
    {
        uiManager.ResetHUD();
        uiManager.HideAllScreens(); // First, hide all pop-up panels
        uiManager.ShowHUD();        // Then, ensure the main game HUD is visible

        levelStartTime = Time.time;
        Time.timeScale = 1f;
        isPaused = false;

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