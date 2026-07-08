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

    public virtual float ModifyOutgoingDamage(float damage) => damage;
    public virtual float ModifyIncomingDamage(float damage) => damage;
    public virtual void OnTurnEnd(ICombatant combatant) => Duration--;
}
