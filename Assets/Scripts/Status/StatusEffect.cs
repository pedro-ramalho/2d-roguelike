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
    public bool IsNegative { get; }

    protected StatusEffect(StatusEffectType type, int duration, bool negative)
    {
        Type = type;
        Duration = duration;
        IsNegative = negative;
    }
    
    public bool IsDepleted => Duration <= 0;

    public virtual float ModifyOutgoingDamage(float damage) => damage;
    public virtual float ModifyIncomingDamage(float damage) => damage;
    public virtual void OnTurnEnd(ICombatant combatant) => Duration--;
    public virtual void OnApplied(ICombatant combatant) {}
    public virtual void OnRemoved(ICombatant combatant) {}
    
    public override string ToString() => Type switch
    {
        StatusEffectType.Empowered => "EMPW",
        StatusEffectType.Stunned => "STUN",
        StatusEffectType.Weak => "WEAK",
        StatusEffectType.Vulnerable => "VULN",
        _ => ""
    };
}
