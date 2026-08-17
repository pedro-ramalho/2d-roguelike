using UnityEngine;

public enum BandType
{
    Tutorial,
    Early,
    Mid,
    Late,
}

[CreateAssetMenu(fileName = "LevelBand", menuName = "Scriptable Objects/LevelBand")]
public class LevelBand : ScriptableObject
{
    public BandType Type;

    [Header("Level Range")]
    public int MinLevel;
    public int MaxLevel;

    [Header("Board")]
    public int BoardWidth;
    public int BoardHeight;

    [Header("Content")]
    public int MinWallCount;
    public int MaxWallCount;
    public LevelEntry<FoodObject>[] FoodEntries;
    public LevelEntry<EnemyController>[] EnemyEntries;
}
