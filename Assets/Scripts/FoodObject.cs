using UnityEngine;

public class FoodObject : CellObject
{
    public override void PlayerEntered()
    {
        Destroy(gameObject);

        Debug.Log("Food increased");
    }
}
