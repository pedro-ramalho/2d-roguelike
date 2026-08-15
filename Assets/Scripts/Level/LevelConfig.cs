using System;
using UnityEngine;

[Serializable]
public struct LevelEntry<T>
{
    public T Prefab;
    public int MinCount;
    public int MaxCount;
}

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Game/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Walls")]
    public int MinWallCount;
    public int MaxWallCount;

    [Header("Food")]
    public LevelEntry<FoodObject>[] FoodEntries;

    [Header("Enemies")]
    public LevelEntry<EnemyController>[] EnemyEntries;
}
