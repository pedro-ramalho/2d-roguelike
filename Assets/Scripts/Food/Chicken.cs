using UnityEngine;

public class Chicken : FoodObject
{
    [SerializeField]
    private int m_AttackPoints = 1;

    [SerializeField]
    private int m_FirstAllowedLevel = 1;

    protected override void ApplyEffect(PlayerController player) =>
        player.Combatant.IncreaseAttack(m_AttackPoints);
}
