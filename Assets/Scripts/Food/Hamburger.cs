public class Hamburger : FoodObject
{
    protected override void ApplyEffect(PlayerController player)
    {
        AudioManager.Instance.PlaySFX(m_ConsumeSFX);
        player.Combatant.ChangeStamina(GetAmountForCurrentBand());
    }
}
