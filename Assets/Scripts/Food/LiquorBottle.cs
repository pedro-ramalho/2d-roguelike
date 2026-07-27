public class LiquorBottle : FoodObject
{
    protected override void ApplyEffect(PlayerController player) => player.PlayerStats.ChangeStamina(5);
}
