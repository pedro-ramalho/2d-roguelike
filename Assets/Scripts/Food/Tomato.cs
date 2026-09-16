using System.Collections.Generic;
using System.Linq;
using Player;
using Status;
using UnityEngine;

namespace Food
{
    public class Tomato : FoodObject
    {
        protected override void ApplyEffect(PlayerController player)
        {
            List<StatusEffect> negativeStatuses = player
                .Combatant.Statuses.StatusEffects.Where(s => s.IsNegative)
                .ToList();

            if (negativeStatuses.Count == 0)
                return;

            StatusEffect randomNegativeEffect = negativeStatuses[
                Random.Range(0, negativeStatuses.Count)
            ];

            player.Combatant.Statuses.Remove(randomNegativeEffect);
        }
    }
}
