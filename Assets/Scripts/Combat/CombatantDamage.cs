using Status;
using UnityEngine;

namespace Combat
{
    public readonly struct DamageResult
    {
        public int BlockLost { get; }
        public int HPLost { get; }

        public DamageResult(int blockLost, int hpLost)
        {
            BlockLost = blockLost;
            HPLost = hpLost;
        }
    }

    public static class CombatantDamage
    {
        public static DamageResult ApplyDamage(Combatant attacker, Combatant defender)
        {
            float damage = attacker.Attack;

            foreach (StatusEffect status in attacker.StatusEffects)
                damage = status.ModifyOutgoingDamage(damage);

            foreach (StatusEffect status in defender.StatusEffects)
                damage = status.ModifyIncomingDamage(damage);

            int finalDamage = Mathf.Max(0, Mathf.FloorToInt(damage));

            DamageResult result = defender.TakeDamage(finalDamage);

            if (result.HPLost > 0)
                attacker.RollStatusOnHit(defender);

            return result;
        }
    }
}
