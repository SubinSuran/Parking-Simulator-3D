using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Level Management")]
    public GameObject[] levelPrefabs; // An array to hold all our level prefabs
    public Transform playerCar;       // A reference to the player's car transform

    [Header("System References")]
    public UIManager uiManager;

    private int currentLevelIndex = 0;
    private GameObject currentLevelInstance;

    void Start()
    {
        // When the game starts, load the very first level
        LoadLevel(currentLevelIndex);
    }

    private void OnEnable()
    {
        ParkingSpace.OnParkedSuccess += HandleParkedSuccess;
    }

    private void OnDisable()
    {
        ParkingSpace.OnParkedSuccess -= HandleParkedSuccess;
    }

    void HandleParkedSuccess()
    {
        Debug.Log("GAME MANAGER: Level Complete!");

        // When a level is won, show the win screen.
        // We will add a "Next Level" button to this screen later.
        uiManager.ShowLevelCompleteScreen();
    }

    // We will call this from a UI button in the future.
    public void LoadNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex < levelPrefabs.Length)
        {
            uiManager.HideLevelCompleteScreen();
            LoadLevel(currentLevelIndex);
        }
        else
        {
            Debug.Log("GAME OVER - ALL LEVELS COMPLETE!");
            uiManager.ShowGameCompleteScreen();
        }
    }

    void LoadLevel(int levelIndex)
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        currentLevelInstance = Instantiate(levelPrefabs[levelIndex], Vector3.zero, Quaternion.identity);

        Transform startPoint = currentLevelInstance.transform.Find("PlayerStartPoint");
        if (startPoint == null)
        {
            Debug.LogError("PlayerStartPoint not found in level prefab: " + levelPrefabs[levelIndex].name);
            return;
        }

        if (playerCar != null)
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