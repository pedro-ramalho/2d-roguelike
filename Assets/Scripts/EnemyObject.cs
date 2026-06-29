using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyObject : CellObject
{
    private int m_HealthPoint;

    public int MaxHealth = 3;
    public int FoodDamage = 5;

    void Awake() => GameManager.Instance.TurnManager.OnTick += TurnHappened;

    void OnDestroy() => GameManager.Instance.TurnManager.OnTick -= TurnHappened;

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

    bool MoveTo(Vector2Int coord)
    {
        BoardManager board = GameManager.Instance.BoardManager;
        BoardManager.CellData targetCell = board.GetCellData(coord);

        if (targetCell == null || !targetCell.Passable || targetCell.ContainedObject != null)
        {
            return false;
        }

        BoardManager.CellData currentCell = board.GetCellData(m_Cell);
        currentCell.ContainedObject = null;

        targetCell.ContainedObject = this;
        m_Cell = coord;
        transform.position = board.CellToWorld(coord);

        return true;
    }

    void TurnHappened()
    {
        
    }
}
