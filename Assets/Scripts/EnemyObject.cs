using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyObject : CellObject
{
    private int m_HealthPoint;
    private Vector3 m_MoveTarget;

    public int MaxHealth = 3;
    public int FoodDamage = 5;
    public float MoveSpeed = 3f;

    void Awake() => GameManager.Instance.TurnManager.OnTick += TurnHappened;

    void OnDestroy() => GameManager.Instance.TurnManager.OnTick -= TurnHappened;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, m_MoveTarget, MoveSpeed * Time.deltaTime);
    }

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);

        m_HealthPoint = MaxHealth;
        m_MoveTarget = GameManager.Instance.BoardManager.CellToWorld(cell);
        
        transform.position = m_MoveTarget;
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
        m_MoveTarget = board.CellToWorld(coord);

        return true;
    }

    void TurnHappened()
    {
        Vector2Int playerCell = GameManager.Instance.PlayerController.Cell;

        int xDist = playerCell.x - m_Cell.x;
        int yDist = playerCell.y - m_Cell.y;

        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        bool isAdjacent = (xDist == 0 && absYDist == 1) ||  (yDist == 0 && absXDist == 1);
        if (isAdjacent)
        {
            GameManager.Instance.ChangeFood(-FoodDamage);
        }
        else
        {
            if (absXDist > absYDist)
            {
                if (!TryMoveInX(xDist))
                {
                    TryMoveInY(yDist);
                }
            }
            else
            {
                if (!TryMoveInY(yDist))
                {
                    TryMoveInX(xDist);
                }
            }
        }
    }

    bool TryMoveInX(int xDist)
    {
        if (xDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.right);
        }

        return MoveTo(m_Cell + Vector2Int.left);
    }

    bool TryMoveInY(int yDist)
    {
        if (yDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.up);
        }

        return MoveTo(m_Cell + Vector2Int.down);
    }
}
