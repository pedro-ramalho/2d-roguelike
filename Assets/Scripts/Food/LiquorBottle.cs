public class LiquorBottle : FoodObject
{
    protected override void ApplyEffect(PlayerController player)
    {
        player.Combatant.IncreaseMaxHP(GetAmountForCurrentBand());
        player.Combatant.Heal(GetAmountForCurrentBand());
    }
}
