using UnityEngine;

public class StatusEffectEmpowered : StatusEffect
{
    private readonly float m_EmpoweredDamageMultiplier = 1.5f;

    public StatusEffectEmpowered(int duration)
        : base(StatusEffectType.Empowered, duration, false) { }

    public override float ModifyOutgoingDamage(float damage) =>
        damage * m_EmpoweredDamageMultiplier;
}
