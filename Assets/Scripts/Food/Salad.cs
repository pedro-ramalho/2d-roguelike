using UnityEngine;

public class Salad : FoodObject
{
    [SerializeField]
    private int m_HealthPoints = 3;

    protected override void ApplyEffect(PlayerController player)
    {
        AudioManager.Instance.PlaySFX(m_ConsumeSFX);
        player.Combatant.Heal(m_HealthPoints);
    }
}
