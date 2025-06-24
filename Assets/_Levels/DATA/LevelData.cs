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
}