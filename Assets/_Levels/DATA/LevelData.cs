using UnityEngine;

// We define the possible objectives here in an enum
public enum LevelObjectiveType
{
    ParkInTime,
    ReverseParkOnly
}

[CreateAssetMenu(fileName = "LevelData", menuName = "Parking Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Setup")]
    public GameObject levelPrefab;

    [Header("Objectives")]
    public LevelObjectiveType objective; // The dropdown to select the objective
    public bool failOnCollision = true;  // Should crashing fail the level?

    [Header("Star Timings (in seconds)")]
    public float timeFor3Stars = 30f;
    public float timeFor2Stars = 60f;
    // --- NEW ---
    [Tooltip("Finish under this time to get at least 1 star.")]
    public float timeFor1Star = 70f;

    [Header("Failure Condition")]
    // --- NEW ---
    [Tooltip("If the player takes longer than this, the level is automatically failed.")]
    public float timeForGameOver = 120f;
}
