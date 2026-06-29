using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyObject : CellObject
{
    private int m_HealthPoint;

    public int MaxHealth = 3;
    public int FoodDamage = 5;


    public override void Init(Vector2Int cell)
    {
        base.Init(cell);

        m_HealthPoint = MaxHealth;
    }

    public override bool PlayerWantsToEnter()
    {
        m_HealthPoint--;

        // Enemy is still not dead, so Player cannot enter the cell yet
        if (m_HealthPoint > 0)
        {
            return false;
        }

        Destroy(gameObject);

        return true;
    }

    public override void PlayerEntered() => GameManager.Instance?.ChangeFood(-FoodDamage);
}
