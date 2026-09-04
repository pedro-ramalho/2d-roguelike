using Player;

namespace Food
{
    public class Salad : FoodObject
    {
        protected override void ApplyEffect(PlayerController player)
        {
            player.Combatant.Heal(GetAmountForCurrentBand());
        }
    }
}
