using System.Collections.Generic;
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
            int finalDamage = ComputeDamage(
                attacker.Stats.Attack,
                attacker.Statuses.StatusEffects,
                defender.Statuses.StatusEffects
            );

            DamageResult result = defender.TakeDamage(finalDamage);

            if (result.HPLost > 0)
                attacker.Statuses.RollStatusOnHit(defender);

            return result;
        }

        public static int ComputeDamage(
            int baseAttack,
            IReadOnlyList<StatusEffect> attackerStatuses,
            IReadOnlyList<StatusEffect> defenderStatuses
        )
        {
            float damage = baseAttack;

            for (int i = 0; i < attackerStatuses.Count; i++)
                damage = attackerStatuses[i].ModifyOutgoingDamage(damage);

            for (int i = 0; i < defenderStatuses.Count; i++)
                damage = defenderStatuses[i].ModifyIncomingDamage(damage);

            return Mathf.Max(0, Mathf.FloorToInt(damage));
        }
    }
}
