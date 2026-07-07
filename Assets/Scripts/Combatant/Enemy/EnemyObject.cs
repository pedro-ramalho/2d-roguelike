using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyObject : CellObject, ICombatant
{
    [SerializeField]
    private int m_MaxHP = 10;
    private CombatantState m_State;

    public int FoodDamage = 5;

    public int MaxHP => m_State.MaxHP;
    public int HP => m_State.HP;
    public int Block => m_State.Block;

    public IReadOnlyList<StatusEffect> StatusEffects => m_State.StatusEffects;


    void Awake()
    {
        m_State = new CombatantState(m_MaxHP);

        GameManager.Instance.TurnManager.OnTick += TurnHappened;
    }

    void OnDestroy() => GameManager.Instance.TurnManager.OnTick -= TurnHappened;

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);

        transform.position = GameManager.Instance.BoardManager.CellToWorld(cell);
    }

    public DamageResult TakeDamage(int amount) => m_State.TakeDamage(amount);
    public void Heal(int amount) => m_State.Heal(amount);
    public void AddBlock(int amount) => m_State.AddBlock(amount);
    public void ApplyStatusEffect(StatusEffect effect) => m_State.ApplyStatusEffect(effect);

    public override bool PlayerWantsToEnter()
    {
        m_State.TakeDamage(1);

        // Enemy is still not dead, so Player cannot enter the cell yet
        if (HP > 0)
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
