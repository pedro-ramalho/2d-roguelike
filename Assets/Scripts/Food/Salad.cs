using UnityEngine;

public class Salad : FoodObject
{
    [SerializeField]
    private int m_HealthPoints = 3;

    [SerializeField]
    private int m_FirstAllowedLevel = 1;

    protected override void ApplyEffect(PlayerController player) =>
        player.Combatant.Heal(m_HealthPoints);
}
