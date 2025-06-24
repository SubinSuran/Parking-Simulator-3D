using UnityEngine;
using UnityEngine.SceneManagement; // Make sure this is at the top

public class GameManager : MonoBehaviour
{
    [Header("Level Management")]
    public GameObject[] levelPrefabs; // Your array of 10 level prefabs
    public Transform playerCar;       // The player's car transform

    [Header("System References")]
    public UIManager uiManager; // The UIManager that controls all UI panels

    // --- Private State Variables ---
    private int currentLevelIndex = 0;
    private GameObject currentLevelInstance;
    private bool isPaused = false;

    // --- Unity Lifecycle Methods ---

    void Start()
    {
        // Load the level that was selected in the Main Menu
        currentLevelIndex = PlayerPrefs.GetInt("SelectedLevelIndex", 0);
        LoadLevel(currentLevelIndex);
    }

    private void OnEnable()
    {
        // Subscribe to events when this object becomes active
        ParkingSpace.OnParkedSuccess += HandleParkedSuccess;
        CollisionHandler.OnPlayerCrashed += HandleGameOver; // Make sure this matches your script name
    }

    private void OnDisable()
    {
        // Unsubscribe from events when this object is disabled to prevent errors
        ParkingSpace.OnParkedSuccess -= HandleParkedSuccess;
        CollisionHandler.OnPlayerCrashed -= HandleGameOver;
    }

    void Update()
    {
        // Listen for the Escape key to pause/unpause the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // --- Event Handlers ---


    void HandleParkedSuccess()
    {
        Debug.Log("GAME MANAGER: Level Complete!");

        // --- THIS IS THE MISSING LINE ---
        SaveManager.LevelCompleted(currentLevelIndex);
        // --- END OF MISSING LINE ---

        uiManager.ShowLevelCompleteScreen();
    }

    void HandleGameOver()
    {
        Debug.Log("GAME MANAGER: Player has crashed. GAME OVER.");
        Time.timeScale = 0f; // Freeze the game
        uiManager.ShowGameOverScreen(); // Tell UIManager to show the Game Over panel
    }

    // --- Pause Logic ---

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
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

    // --- UI Button Functions ---

    public void RestartLevel()
    {
        uiManager.HideAllScreens(); // Hide the Game Over or Pause panel
        LoadLevel(currentLevelIndex); // Reload the current level
    }

    public void LoadNextLevel()
    {
        uiManager.HideAllScreens();
        currentLevelIndex++;
        if (currentLevelIndex < levelPrefabs.Length)
        {
            LoadLevel(currentLevelIndex);
        }
        else
        {
            Debug.Log("GAME OVER - ALL LEVELS COMPLETE!");
            uiManager.ShowGameCompleteScreen();
        }
    }

    public void GoToMainMenu()
    {
        // IMPORTANT: Always reset time scale before changing scenes
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // --- Core Level Loading ---

    void LoadLevel(int levelIndex)
    {
        // 1. Reset Time Scale
        Time.timeScale = 1f;

        // 2. Reset the car's crash state for the new attempt
        if (playerCar != null)
        {
            CollisionHandler carCollision = playerCar.GetComponent<CollisionHandler>();
            if (carCollision != null)
            {
                carCollision.ResetCrashState();
            }
        }

        // 3. Destroy the old level and instantiate the new one
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }
        currentLevelInstance = Instantiate(levelPrefabs[levelIndex], Vector3.zero, Quaternion.identity);

        // 4. Find the start point and move the car
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
            Debug.LogError("PlayerStartPoint not found in level prefab: " + levelPrefabs[levelIndex].name);
        }
    }
}