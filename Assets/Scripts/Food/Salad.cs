public class Salad : FoodObject
{
    protected override void ApplyEffect(PlayerController player)
    {
        AudioManager.Instance.PlaySFX(m_ConsumeSFX);
        player.Combatant.Heal(GetAmountForCurrentBand());
    }
}
