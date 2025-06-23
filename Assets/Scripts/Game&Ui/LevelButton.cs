using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI levelText;
    public GameObject lockIcon;

    // --- Private variables ---
    private Button button;
    private int levelIndex;
    private MenuManager menuManager;

    // Awake is called when the object is first created
    private void Awake()
    {
        // Automatically find the Button component on this same GameObject
        button = GetComponent<Button>();
    }

    public void Setup(int index, MenuManager manager)
    {
        levelIndex = index;
        menuManager = manager;
        levelText.text = (levelIndex + 1).ToString();
    }

    public void HandleClick()
    {
        // NEW LINE FOR DEBUGGING
        Debug.Log("Button clicked for level index: " + levelIndex + ". Attempting to load level.");

        menuManager.OnLevelSelected(levelIndex);
    }

    public void SetLocked(bool isLocked)
    {
        button.interactable = !isLocked;
        if (lockIcon != null)
        {
            lockIcon.SetActive(isLocked);
        }
    }
}