using UnityEngine;

public static class CombatantDamage
{
    public static DamageResult ApplyDamage(ICombatant attacker, ICombatant defender)
    {
        return defender.TakeDamage(attacker.Attack);
    }
}
