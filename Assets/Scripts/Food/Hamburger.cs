using Player;

namespace Food
{
    public class Hamburger : FoodObject
    {
        protected override void ApplyEffect(PlayerController player)
        {
            player.Combatant.Stats.ChangeStamina(GetAmountForCurrentBand());
        }
    }
}
