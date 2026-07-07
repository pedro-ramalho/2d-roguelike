using UnityEngine;

public enum StatusEffectType
{
    Empowered,
    Vulnerable,
    Weak,
    Stunned    
}

public abstract class StatusEffect
{
    public int Duration { get; set; }
    public StatusEffectType Type { get; }

    protected StatusEffect(StatusEffectType type)
    {
        Type = type;
    }
}
