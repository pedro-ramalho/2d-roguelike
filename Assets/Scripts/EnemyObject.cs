using UnityEngine;

public class EnemyObject : CellObject
{
    public int FoodDamage = 5;

    public override void PlayerEntered() => GameManager.Instance?.ChangeFood(-FoodDamage);
}
