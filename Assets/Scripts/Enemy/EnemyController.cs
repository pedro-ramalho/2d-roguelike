using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // References
    private BoardManager m_Board;
    private EnemyObject m_Combatant;
    private CombatantAnimator m_CombatantAnimator;

    // State
    private Vector2Int m_Cell;

    public EnemyObject Combatant => m_Combatant;
    public Vector2Int Cell => m_Cell;
    
    void Awake()
    {
        m_Board = GameManager.Instance.BoardManager;
        m_Combatant = GetComponent<EnemyObject>();
        m_CombatantAnimator = GetComponent<CombatantAnimator>();    
    }

    void Start()
    {
        GameManager.Instance.TurnManager.OnTick += OnTurnHappened;
    }

    void OnDestroy()
    {
        GameManager.Instance.TurnManager.OnTick += OnTurnHappened;
    }

    bool MoveTo(Vector2Int coord)
    {
        BoardManager.CellData targetCell = m_Board.GetCellData(coord);

        bool isInvalidCell = (targetCell == null) || (!targetCell.Passable) || (targetCell.ContainedObject != null);
        if (isInvalidCell)
            return false;

        BoardManager.CellData currentCell = m_Board.GetCellData(m_Cell);
        currentCell.ContainedObject = null;
        
        targetCell.ContainedObject = m_Combatant;
        m_Cell = coord;

        m_CombatantAnimator.PlayWalkAnimation(coord);

        return true;
    }

    bool TryMove(Vector2Int direction) => MoveTo(m_Cell + direction);

    bool IsAdjacentToPlayer(Vector2Int delta) => Mathf.Abs(delta.x) + Mathf.Abs(delta.y) == 1;
    
    void MoveTowards(Vector2Int delta)
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

    void OnTurnHappened()
    {
        if (m_Combatant.IsStunned)
            return;

        Vector2Int delta = GameManager.Instance.PlayerController.Cell - m_Cell;

        if (IsAdjacentToPlayer(delta))
        {
            CombatantDamage.ApplyDamage(m_Combatant, GameManager.Instance.PlayerController.Combatant);
            
            return;
        }
        
        MoveTowards(delta);
    }
}
