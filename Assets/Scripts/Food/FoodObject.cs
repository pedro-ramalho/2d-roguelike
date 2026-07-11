using UnityEngine;

public class FoodObject : CellObject
{
    public int Points = 5;
    
    public override void PlayerEntered(Player player)
    {
        Destroy(gameObject);

        player.ChangeStamina(Points);
    }
}
