using UnityEngine;

public class Hamburger : FoodObject
{
    [SerializeField]
    private int m_StaminaPoints = 5;

    protected override void ApplyEffect(PlayerController player) =>
        player.Combatant.ChangeStamina(m_StaminaPoints);
}
