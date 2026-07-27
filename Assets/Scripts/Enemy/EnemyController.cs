using System;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    // Serialized references
    [SerializeField] private StatusEffectRoll[] m_StatusRolls;

    // Private references
    private BoardManager m_BoardManager;
    private TurnManager m_TurnManager;
    private PlayerController m_PlayerController;
    private Combatant m_Combatant;
    private CombatantAnimator m_CombatantAnimator;

    // State
    private Vector2Int m_Cell;
    private int m_Level;

    // Protected properties
    protected CombatantAnimator Animator => m_CombatantAnimator;
    protected Vector2Int PlayerCell => m_PlayerController.Cell;

    // Public properties
    public Combatant Combatant => m_Combatant;
    public Vector2Int Cell => m_Cell;
    
    void Awake()
    {
        m_Combatant = GetComponent<Combatant>();
        m_CombatantAnimator = GetComponent<CombatantAnimator>();

        m_Combatant.Defeated += OnDefeated;
    }

    void Start() => m_TurnManager.OnTick += OnTurnHappened;
    
    void OnDestroy()
    {
        if (m_TurnManager != null)
            m_TurnManager.OnTick -= OnTurnHappened;
        
        if (m_Combatant != null)
            m_Combatant.Defeated -= OnDefeated;
    }

    void OnDefeated()
    {
        if (m_TurnManager != null)
            m_TurnManager.OnTick -= OnTurnHappened;
    }

    bool MoveTo(Vector2Int coord)
    {
        BoardManager.CellData targetCell = m_BoardManager.GetCellData(coord);

        bool isInvalidCell = (targetCell == null) || (!targetCell.Passable) || (targetCell.ContainedObject != null);
        if (isInvalidCell)
            return false;

        BoardManager.CellData currentCell = m_BoardManager.GetCellData(m_Cell);
        currentCell.ContainedObject = null;
        
        targetCell.ContainedObject = m_Combatant;
        m_Cell = coord;

        m_CombatantAnimator.PlayWalkAnimation(coord);

        return true;
    }

    void SnapTo(Vector2Int coord)
    {
        m_Cell = coord;
        transform.position = m_BoardManager.CellToWorld(coord);
    }

    protected bool CanEnterCell(Vector2Int coord)
    {
        BoardManager.CellData targetCell = m_BoardManager.GetCellData(coord);

        bool isInvalidCell = (targetCell == null) || (!targetCell.Passable) || (targetCell.ContainedObject != null);
        if (isInvalidCell)
            return false;

        return true;
    }

    protected bool TryMove(Vector2Int direction) => MoveTo(m_Cell + direction);
    
    protected bool IsInLineOfSightToPlayer() => m_BoardManager.IsInLineOfSight(m_Cell, m_PlayerController.Cell);

    protected bool IsAdjacentToPlayer(Vector2Int delta) => Mathf.Abs(delta.x) + Mathf.Abs(delta.y) == 1;
    
    protected void MoveTowards(Vector2Int delta)
    {
        Vector2Int xDirection = delta.x > 0 ? Vector2Int.right : Vector2Int.left;
        Vector2Int yDirection = delta.y > 0 ? Vector2Int.up : Vector2Int.down;

        bool prioritizeX = Mathf.Abs(delta.x) > Mathf.Abs(delta.y);

        if (prioritizeX)
        {
            if (!TryMove(xDirection))
                TryMove(yDirection);
        }
        else
        {
            if (!TryMove(yDirection))
                TryMove(xDirection);
        }
    }

    protected Vector3 PlayerCellToWorld() => m_BoardManager.CellToWorld(PlayerCell);

    protected Vector2Int ComputeDeltaToPlayer() => m_PlayerController.Cell - m_Cell;

    protected void DealDamageToPlayer() 
    {
        DamageResult result = m_Combatant.DealDamageTo(m_PlayerController.Combatant);
        if (result.HPLost > 0)
            TryApplyStatusToPlayer();
    }
    
    protected void AttackPlayer(Vector2Int direction)
    {
        DamageResult result = m_Combatant.AttackTarget(m_PlayerController.Combatant, new Vector2Int(Math.Sign(direction.x), Math.Sign(direction.y)));
        if (result.HPLost > 0)
            TryApplyStatusToPlayer();  
    } 

    protected void ChaseOrAttack(Vector2Int delta)
    {
        if (IsAdjacentToPlayer(delta))
            AttackPlayer(delta);
        else
            MoveTowards(delta);
    }

    protected void TryApplyStatusToPlayer()
    {
        foreach (StatusEffectRoll roll in m_StatusRolls)
        {
            float probability = Mathf.Clamp01(roll.BaseProbability + roll.PerLevelBonus * m_Level);
            if (UnityEngine.Random.value < probability)
            {
                m_PlayerController.Combatant.ApplyStatusEffect(StatusEffectFactory.FromType(roll.Type, roll.Duration));

                return;
            }
        }
    }

    void OnTurnHappened()
    {
        if (m_Combatant.IsStunned)
            return;

        ResolveEnemyAction();
    }

    protected abstract void ResolveEnemyAction();

    public void Spawn(BoardManager boardManager, TurnManager turnManager, PlayerController playerController, Vector2Int cell, int currentLevel)
    {
        m_BoardManager = boardManager;
        m_TurnManager = turnManager;
        m_PlayerController = playerController;
        m_Level = currentLevel;

        SnapTo(cell);
    }
}
