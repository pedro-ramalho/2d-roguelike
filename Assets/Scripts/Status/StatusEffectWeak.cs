using UnityEngine;

public class StatusEffectWeak : StatusEffect
{
    private readonly float m_WeakDamageMultiplier = 0.5f;

    public StatusEffectWeak(int duration)
        : base(StatusEffectType.Weak, duration, true) { }

    public override float ModifyOutgoingDamage(float damage) => damage * m_WeakDamageMultiplier;
}
