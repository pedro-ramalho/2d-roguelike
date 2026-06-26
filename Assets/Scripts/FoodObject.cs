using UnityEngine;

public class FoodObject : CellObject
{
    public int Points = 5;
    public override void PlayerEntered()
    {
        Destroy(gameObject);

        GameManager.Instance?.ChangeFood(Points);
    }
}
