using Player;

namespace Food
{
    public class Chicken : FoodObject
    {
        protected override void ApplyEffect(PlayerController player) =>
            player.Combatant.Stats.UpgradeStat(
                Combat.CombatantStat.Attack,
                GetAmountForCurrentBand()
            );
    }
}
