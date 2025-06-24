using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Parking Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Tooltip("The prefab that contains the level's layout.")]
    public GameObject levelPrefab;

    [Header("Star Timings (in seconds)")]
    [Tooltip("Finish under this time to get 3 stars.")]
    public float timeFor3Stars = 30f;

    [Tooltip("Finish under this time to get 2 stars.")]
    public float timeFor2Stars = 60f;
}