using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "LevelProgressionSettings",
    menuName = "Game/Level Progression Settings"
)]
public class LevelProgressionSettings : ScriptableObject
{
    public LevelBand[] Bands;

    [Header("Elite")]
    public int EliteCadence = 3;
    public EliteRewardEntry[] EliteRewardPool;

    [Header("Status")]
    [Range(0f, 1f)]
    public float MaxStatusChance = 0.5f;
}
