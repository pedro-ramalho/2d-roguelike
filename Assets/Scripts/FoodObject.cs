using UnityEngine;

public class FoodObject : CellObject
{
    public int Points { get; private set; } = 5;
    public override void PlayerEntered()
    {
        Destroy(gameObject);

        GameManager.Instance?.ChangeFood(Points);
    }
}
