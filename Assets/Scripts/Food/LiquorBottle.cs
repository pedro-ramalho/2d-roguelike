using Player;

namespace Food
{
    public class LiquorBottle : FoodObject
    {
        protected override void ApplyEffect(PlayerController player)
        {
            player.Combatant.Stats.UpgradeStat(
                Combat.CombatantStat.MaxHP,
                GetAmountForCurrentBand()
            );
            player.Combatant.Stats.Heal(GetAmountForCurrentBand());
        }
    }
}
