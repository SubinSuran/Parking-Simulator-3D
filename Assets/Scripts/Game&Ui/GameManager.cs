using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Level Management")]
    // This is now an array of LevelData, not GameObjects
    public LevelData[] levels;
    public Transform playerCar;

    [Header("System References")]
    public UIManager uiManager;

    // --- Private State Variables ---
    private int currentLevelIndex = 0;
    private GameObject currentLevelInstance;
    private bool isPaused = false;
    private float levelStartTime; // The stopwatch for the level

    // --- Unity Lifecycle Methods ---

    void Start()
    {
        currentLevelIndex = PlayerPrefs.GetInt("SelectedLevelIndex", 0);
        LoadLevel(currentLevelIndex);
    }

    private void OnEnable()
    {
        ParkingSpace.OnParkedSuccess += HandleParkedSuccess;
        CollisionHandler.OnPlayerCrashed += HandleGameOver;
    }

    private void OnDisable()
    {
        ParkingSpace.OnParkedSuccess -= HandleParkedSuccess;
        CollisionHandler.OnPlayerCrashed -= HandleGameOver;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // --- Event Handlers ---

    // THIS IS THE NEW, UPGRADED METHOD
    void HandleParkedSuccess()
    {
        // 1. Calculate how long the level took
        float timeTaken = Time.time - levelStartTime;
        int starsEarned = 0;

        // 2. Get the star thresholds from the current level's data
        LevelData currentLevelData = levels[currentLevelIndex];

        // 3. Compare the time taken to the thresholds to award stars
        if (timeTaken <= currentLevelData.timeFor3Stars)
        {
            starsEarned = 3;
        }
        else if (timeTaken <= currentLevelData.timeFor2Stars)
        {
            starsEarned = 2;
        }
        else
        {
            starsEarned = 1; // 1 star just for completing the level
        }

        Debug.Log("Level Complete! Time: " + timeTaken + "s, Stars: " + starsEarned);

        // 4. Save the progress
        SaveManager.LevelCompleted(currentLevelIndex); // This unlocks the next level
        SaveManager.SaveStars(currentLevelIndex, starsEarned); // We will add this to SaveManager next

        // 5. Tell the UIManager to show the win screen AND how many stars to display
        uiManager.ShowLevelCompleteScreen(starsEarned);
    }

    void HandleGameOver()
    {
        Time.timeScale = 0f;
        uiManager.ShowGameOverScreen();
    }

    // --- Level Loading ---

    void LoadLevel(int levelIndex)
    {
        // Start the stopwatch for the new level
        levelStartTime = Time.time;

        Time.timeScale = 1f;

        if (playerCar != null)
        {
            CollisionHandler carCollision = playerCar.GetComponent<CollisionHandler>();
            if (carCollision != null)
            {
                carCollision.ResetCrashState();
            }
        }

        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        // Make sure to get the prefab from inside the LevelData object
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
        else
        {
            Debug.LogError("PlayerStartPoint not found in level prefab: " + levels[levelIndex].levelPrefab.name);
        }
    }

    // --- All other functions (Pause, Restart, NextLevel, etc.) remain the same ---
    #region UI and Pause Logic
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused) { PauseGame(); }
        else { ResumeGame(); }
    }
    void PauseGame()
    {
        Time.timeScale = 0f;
        uiManager.ShowPauseMenu();
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        uiManager.HidePauseMenu();
        isPaused = false;
    }
    public void RestartLevel()
    {
        uiManager.HideAllScreens();
        LoadLevel(currentLevelIndex);
    }
    public void LoadNextLevel()
    {
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
    }
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    #endregion
}