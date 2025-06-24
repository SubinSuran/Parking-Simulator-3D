using UnityEngine;

public static class SaveManager
{
    // Key for the highest level unlocked
    private const string HighestLevelKey = "HighestLevelUnlocked";
    // Base key for stars. We'll add the level index to it, e.g., "Level_0_Stars"
    private const string StarKeyPrefix = "Level_";

    /// <summary>
    /// Unlocks the next level if the completed level was the previous highest.
    /// </summary>
    public static void LevelCompleted(int completedLevelIndex)
    {
        int levelNumberJustCompleted = completedLevelIndex + 1;
        int previouslyUnlocked = LoadHighestLevelUnlocked();

        if (levelNumberJustCompleted >= previouslyUnlocked)
        {
            int newHighestLevel = levelNumberJustCompleted + 1;
            PlayerPrefs.SetInt(HighestLevelKey, newHighestLevel);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Loads the highest level number the player is allowed to play.
    /// </summary>
    public static int LoadHighestLevelUnlocked()
    {
        // Defaults to 1 for first-time players
        return PlayerPrefs.GetInt(HighestLevelKey, 1);
    }

    /// <summary>
    /// Saves the star count for a specific level, only if it's a new high score.
    /// </summary>
    public static void SaveStars(int levelIndex, int starCount)
    {
        string key = StarKeyPrefix + levelIndex + "_Stars";
        // Only save if the new star count is greater than the previously saved one.
        if (starCount > LoadStars(levelIndex))
        {
            PlayerPrefs.SetInt(key, starCount);
            PlayerPrefs.Save();
            Debug.Log("Saved " + starCount + " stars for Level " + (levelIndex + 1));
        }
    }

    /// <summary>
    /// Loads the saved star count for a specific level.
    /// </summary>
    public static int LoadStars(int levelIndex)
    {
        string key = StarKeyPrefix + levelIndex + "_Stars";
        // Defaults to 0 if no stars have been earned yet for this level.
        return PlayerPrefs.GetInt(key, 0);
    }
}