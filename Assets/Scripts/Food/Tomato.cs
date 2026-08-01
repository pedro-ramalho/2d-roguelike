using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tomato : FoodObject
{
    [SerializeField]
    private int m_FirstAllowedLevel = 1;

    protected override void ApplyEffect(PlayerController player)
    {
        List<StatusEffect> negativeStatuses = player
            .Combatant.StatusEffects.Where(s => s.IsNegative)
            .ToList();

        if (negativeStatuses.Count == 0)
            return;

        StatusEffect randomNegativeEffect = negativeStatuses[
            Random.Range(0, negativeStatuses.Count)
        ];

        player.Combatant.RemoveStatusEffect(randomNegativeEffect);
    }
}
