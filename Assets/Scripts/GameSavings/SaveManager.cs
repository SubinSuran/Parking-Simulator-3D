// File: SaveManager.cs
using UnityEngine;

public static class SaveManager
{
    private const string HighestLevelKey = "HighestLevelUnlocked";

    public static void LevelCompleted(int completedLevelIndex)
    {
        int levelNumberJustCompleted = completedLevelIndex + 1;
        int previouslyUnlocked = LoadHighestLevelUnlocked();

        if (levelNumberJustCompleted >= previouslyUnlocked)
        {
            int newHighestLevel = levelNumberJustCompleted + 1;
            PlayerPrefs.SetInt(HighestLevelKey, newHighestLevel);
            PlayerPrefs.Save();
            Debug.Log("PROGRESS SAVED: New highest level unlocked is " + newHighestLevel);
        }
    }

    public static int LoadHighestLevelUnlocked()
    {
        return PlayerPrefs.GetInt(HighestLevelKey, 1); // Default to 1 for first-time players
    }
}