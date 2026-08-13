using System;
using UnityEngine;

[Serializable]
public class LevelBand
{
    public string Name;
    public int MinLevel;
    public int MaxLevel;
    public int BoardWidth;
    public int BoardHeight;
    public LevelConfig DefaultConfig;
}

[CreateAssetMenu(
    fileName = "LevelProgressionSettings",
    menuName = "Game/Level Progression Settings"
)]
public class LevelProgressionSettings : ScriptableObject
{
    [Header("Elite Cadence")]
    public int EliteCadence = 3;

    [Header("Status Rolls")]
    [Range(0f, 1f)]
    public float MaxStatusChance = 1f;

    [Header("Level Bands")]
    public LevelBand[] LevelBands;

    [Header("Elite Rewards")]
    public EliteRewardEntry[] EliteRewardPool;
}
