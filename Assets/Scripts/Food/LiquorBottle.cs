using UnityEngine;

public class LiquorBottle : FoodObject
{
    [SerializeField]
    private int m_MaxHealthPoints = 1;

    [SerializeField]
    private int m_HealthPoints = 1;

    protected override void ApplyEffect(PlayerController player)
    {
        AudioManager.Instance.PlaySFX(m_ConsumeSFX);
        player.Combatant.IncreaseMaxHP(m_MaxHealthPoints);
        player.Combatant.Heal(m_HealthPoints);
    }
}
