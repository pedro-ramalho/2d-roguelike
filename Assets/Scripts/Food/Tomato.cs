using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tomato : FoodObject
{
    protected override void ApplyEffect(PlayerController player)
    {
        AudioManager.Instance.PlaySFX(m_ConsumeSFX);
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
