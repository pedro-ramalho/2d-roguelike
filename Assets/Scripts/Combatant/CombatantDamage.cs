using UnityEngine;

public static class CombatantDamage
{
    public static DamageResult ApplyDamage(ICombatant attacker, ICombatant defender)
    {
        float damage = attacker.Attack;

        foreach (StatusEffect status in attacker.StatusEffects)
            damage = status.ModifyOutgoingDamage(damage);

        foreach (StatusEffect status in defender.StatusEffects)
            damage = status.ModifyIncomingDamage(damage);

        return defender.TakeDamage(Mathf.FloorToInt(damage));
    }
}
