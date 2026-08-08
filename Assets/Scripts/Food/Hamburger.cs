using UnityEngine;

public class Hamburger : FoodObject
{
    [SerializeField]
    private int m_StaminaPoints = 5;

    protected override void ApplyEffect(PlayerController player)
    {
        AudioManager.Instance.PlaySFX(m_ConsumeSFX);
        player.Combatant.ChangeStamina(m_StaminaPoints);
    }
}
