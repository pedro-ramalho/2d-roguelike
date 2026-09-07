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
            float damage = attacker.Stats.Attack;

            foreach (StatusEffect status in attacker.Statuses.StatusEffects)
                damage = status.ModifyOutgoingDamage(damage);

            foreach (StatusEffect status in defender.Statuses.StatusEffects)
                damage = status.ModifyIncomingDamage(damage);

            int finalDamage = Mathf.Max(0, Mathf.FloorToInt(damage));

            DamageResult result = defender.TakeDamage(finalDamage);

            if (result.HPLost > 0)
                attacker.Statuses.RollStatusOnHit(defender);

            return result;
        }
    }
}
