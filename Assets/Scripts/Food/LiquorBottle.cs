public class LiquorBottle : FoodObject
{
    protected override void ApplyEffect(PlayerController player)
    {
        AudioManager.Instance.PlaySFX(m_ConsumeSFX);
        player.Combatant.IncreaseMaxHP(GetAmountForCurrentBand());
        player.Combatant.Heal(GetAmountForCurrentBand());
    }
}
