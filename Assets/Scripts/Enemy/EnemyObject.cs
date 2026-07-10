using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyObject : CellObject, ICombatant
{
    [SerializeField] private int m_MaxHP = 1000;
    [SerializeField] private int m_Attack = 2;
    private CombatantState m_State;

    public int Attack => m_State.Attack;
    public int MaxHP => m_State.MaxHP;
    public int HP => m_State.HP;
    public int Block => m_State.Block;

    public IReadOnlyList<StatusEffect> StatusEffects => m_State.StatusEffects;


    void Awake()
    {
        m_State = new CombatantState(m_MaxHP, m_Attack);

        GameManager.Instance.TurnManager.OnTick += TurnHappened;
        GameManager.Instance.TurnManager.OnTick += TickStatusEffects;
    }

    void OnDestroy()
    {
        GameManager.Instance.TurnManager.OnTick -= TurnHappened;
        GameManager.Instance.TurnManager.OnTick -= TickStatusEffects;
    }

    void TickStatusEffects() => m_State.TickStatusEffects(this);

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);

        transform.position = GameManager.Instance.BoardManager.CellToWorld(cell);
    }

    public DamageResult TakeDamage(int amount) => m_State.TakeDamage(amount);
    public void Heal(int amount) => m_State.Heal(amount);
    public void AddBlock(int amount) => m_State.AddBlock(amount);
    public void ApplyStatusEffect(StatusEffect effect) => m_State.ApplyStatusEffect(effect, this);
    
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
            CombatantDamage.ApplyDamage(this, GameManager.Instance.PlayerController.Combatant);
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
