using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // References
    private BoardManager m_Board;
    private PlayerInputActions m_InputActions;
    private Player m_Combatant;

    // State
    private Vector2Int m_CellPosition;
    private bool m_IsGameOver;
    
    public Player Combatant => m_Combatant;
    public Vector2Int Cell => m_CellPosition;

    private void Awake()
    {
        m_InputActions = new PlayerInputActions();
        m_Combatant = GetComponent<Player>();
    }

    private void OnEnable() => m_InputActions.Player.Enable();
    private void OnDisable() => m_InputActions.Player.Disable();
    private void OnDestroy() => m_InputActions.Dispose();

    public void Init() => m_IsGameOver = false;

    private void Update()
    {
        if (m_IsGameOver) 
        { 
            HandleRestartInput(); 
            
            return; 
        }
        
        HandleMovementInput();
    }

    private void HandleRestartInput()
    {
        if (m_InputActions.Player.Restart.WasPressedThisFrame()) GameManager.Instance.StartNewGame();
    }

    private Vector2Int GetInputDirection()
    {
        if (m_InputActions.Player.MoveUp.WasPressedThisFrame()) return Vector2Int.up;
        if (m_InputActions.Player.MoveDown.WasPressedThisFrame()) return Vector2Int.down;
        if (m_InputActions.Player.MoveLeft.WasPressedThisFrame()) return Vector2Int.left;
        if (m_InputActions.Player.MoveRight.WasPressedThisFrame()) return Vector2Int.right;

        return Vector2Int.zero;
    }

    private void HandleEnemyDamage(ICombatant enemy, Vector2Int target, BoardManager.CellData cell)
    {
        CombatantDamage.ApplyDamage(m_Combatant, enemy);

        // Enemy has been killed, destroy it and move the Player
        if (enemy.HP <= 0)
        {
            Destroy(cell.ContainedObject.gameObject);
            
            cell.ContainedObject = null;
            
            MoveTo(target);
        }
    }

    private void HandleMovementInput()
    {
        Vector2Int direction = GetInputDirection();
        if (direction == Vector2Int.zero) return;

        Vector2Int target = m_CellPosition + direction;
        BoardManager.CellData cellData = m_Board.GetCellData(target);
        if (cellData == null || !cellData.Passable) return;

        GameManager.Instance.TurnManager.Tick();

        if (cellData.ContainedObject == null)
        {
            MoveTo(target);
        }
        else if (cellData.ContainedObject is ICombatant enemy)
        {
            HandleEnemyDamage(enemy, target, cellData);
        }
        else if (cellData.ContainedObject.PlayerWantsToEnter())
        {
            MoveTo(target);
            cellData.ContainedObject.PlayerEntered();
        }
    }

    public void GameOver() => m_IsGameOver = true;

    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_Board = boardManager;
        MoveTo(cell);
    }

    public void MoveTo(Vector2Int cell)
    {
        m_CellPosition = cell;
        transform.position = m_Board.CellToWorld(cell);
    }
}
