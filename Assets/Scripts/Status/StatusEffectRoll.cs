using UnityEngine;

[System.Serializable]
public class StatusEffectRoll
{
    public StatusEffectType Type;
    public int Duration;

    [Range(0f, 1f)]
    public float BaseProbability;

    [Range(0f, 1f)]
    public float PerLevelBonus;
}
