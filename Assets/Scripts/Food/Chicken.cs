using Player;

namespace Food
{
    public class Chicken : FoodObject
    {
        protected override void ApplyEffect(PlayerController player)
        {
            player.Combatant.IncreaseAttack(GetAmountForCurrentBand());
        }
    }
}
