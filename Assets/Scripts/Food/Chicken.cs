using UnityEngine;

public class Chicken : FoodObject
{
    [SerializeField] private int m_EffectDuration = 3;

    protected override void ApplyEffect(PlayerController player) => player.Combatant.ApplyStatusEffect(StatusEffectFactory.FromType(StatusEffectType.Empowered, m_EffectDuration));
}
