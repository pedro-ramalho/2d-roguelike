using UnityEngine;

public class Tomato : FoodObject
{
    protected override void ApplyEffect(PlayerController player) => player.PlayerStats.ChangeStamina(5);
}
