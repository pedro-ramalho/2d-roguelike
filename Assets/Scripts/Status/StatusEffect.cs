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

    protected StatusEffect(StatusEffectType type, int duration)
    {
        Type = type;
        Duration = duration;
    }

    public bool IsDepleted => Duration <= 0;

    public virtual float ModifyOutgoingDamage(float damage) => damage;
    public virtual float ModifyIncomingDamage(float damage) => damage;
    public virtual void OnTurnEnd(ICombatant combatant) => Duration--;
    public virtual void OnApplied(ICombatant combatant) {}
    public virtual void OnRemoved(ICombatant combatant) {}
}
