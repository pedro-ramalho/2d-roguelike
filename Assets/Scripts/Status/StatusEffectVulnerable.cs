using UnityEngine;

public class StatusEffectVulnerable : StatusEffect
{
    private readonly float m_VulnerableDamageMultiplier = 1.5f;

    public StatusEffectVulnerable(int duration) : base(StatusEffectType.Vulnerable, duration)
    { }

    public override float ModifyIncomingDamage(float damage) => damage * m_VulnerableDamageMultiplier;
}
