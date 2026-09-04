using Player;

namespace Food
{
    public class Fish : FoodObject
    {
        protected override void ApplyEffect(PlayerController player)
        {
            player.Combatant.AddBlock(GetAmountForCurrentBand());
        }
    }
}
