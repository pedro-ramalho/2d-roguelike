using UnityEngine;

public class StatusEffectWeak : StatusEffect
{
    private readonly float m_WeakDamageMultiplier = 0.5f;
    public StatusEffectWeak(StatusEffectType type, int duration) : base(StatusEffectType.Weak, duration)
    { }

    public override float ModifyOutgoingDamage(float damage) => damage * m_WeakDamageMultiplier;
}
