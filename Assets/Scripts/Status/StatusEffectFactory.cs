using UnityEngine;

public static class StatusEffectFactory
{
    public static StatusEffect FromType(StatusEffectType type, int duration)
    {
        return type switch
        {
            StatusEffectType.Stunned => new StatusEffectStunned(duration),
            StatusEffectType.Weak => new StatusEffectWeak(duration),
            StatusEffectType.Vulnerable => new StatusEffectVulnerable(duration),
            StatusEffectType.Empowered => new StatusEffectEmpowered(duration),
            _ => throw new System.ArgumentException($"Unknown status type: {type}")
        };
    }
}
