using Player;

namespace Food
{
    public class LiquorBottle : FoodObject
    {
        protected override void ApplyEffect(PlayerController player)
        {
            player.Combatant.UpgradeStat(Combat.CombatantStat.MaxHP, GetAmountForCurrentBand());
            player.Combatant.Heal(GetAmountForCurrentBand());
        }
    }
}
