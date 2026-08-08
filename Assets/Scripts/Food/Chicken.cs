using UnityEngine;

public class Chicken : FoodObject
{
    [SerializeField]
    private int m_AttackPoints = 1;

    protected override void ApplyEffect(PlayerController player)
    {
        AudioManager.Instance.PlaySFX(m_ConsumeSFX);

        player.Combatant.IncreaseAttack(m_AttackPoints);
    }
}
