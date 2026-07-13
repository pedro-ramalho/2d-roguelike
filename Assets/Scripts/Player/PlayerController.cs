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

    void Start() => GameManager.Instance.TurnManager.OnTick += TurnHappened;
    
    void Awake()
    {
        m_InputActions = new PlayerInputActions();
        m_Combatant = GetComponent<Player>();
    }

    void OnEnable() => m_InputActions.Player.Enable();
    
    void OnDisable() => m_InputActions.Player.Disable();
    
    void OnDestroy()
    {
        m_InputActions.Dispose();

        if (GameManager.Instance != null)
            GameManager.Instance.TurnManager.OnTick -= TurnHappened;        
    } 

    void Update()
    {
        if (m_IsGameOver) 
        { 
            HandleRestartInput(); 
            
            return; 
        }
        
        // Temporary debug logs (should be removed later)
        if (Keyboard.current.vKey.wasPressedThisFrame)
            m_Combatant.ApplyStatusEffect(new StatusEffectEmpowered(2));

        if (Keyboard.current.bKey.wasPressedThisFrame)
            m_Combatant.ApplyStatusEffect(new StatusEffectVulnerable(3));

        if (Keyboard.current.nKey.wasPressedThisFrame)
            m_Combatant.ApplyStatusEffect(new StatusEffectWeak(4));

        if (Keyboard.current.mKey.wasPressedThisFrame)
            m_Combatant.ApplyStatusEffect(new StatusEffectStunned(1));

        if (Keyboard.current.oKey.wasPressedThisFrame)
            m_Combatant.TakeDamage(1);

        if (Keyboard.current.pKey.wasPressedThisFrame)
            m_Combatant.Heal(1);

        HandleMovementInput();
    }

    void HandleRestartInput()
    {
        if (m_InputActions.Player.Restart.WasPressedThisFrame()) 
            GameManager.Instance.StartNewGame();
    }

    Vector2Int GetInputDirection()
    {
        if (m_InputActions.Player.MoveUp.WasPressedThisFrame()) return Vector2Int.up;
        if (m_InputActions.Player.MoveDown.WasPressedThisFrame()) return Vector2Int.down;
        if (m_InputActions.Player.MoveLeft.WasPressedThisFrame()) return Vector2Int.left;
        if (m_InputActions.Player.MoveRight.WasPressedThisFrame()) return Vector2Int.right;

        return Vector2Int.zero;
    }

    void HandleEnemyDamage(ICombatant enemy, Vector2Int target, BoardManager.CellData cell)
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

    void HandleMovementInput()
    {
        Vector2Int direction = GetInputDirection();
        if (direction != Vector2Int.zero) Debug.Log($"direction={direction}");

        if (direction == Vector2Int.zero) return;

        Vector2Int target = m_CellPosition + direction;
        
        BoardManager.CellData cellData = m_Board.GetCellData(target);
        Debug.Log($"target={target} cellData={(cellData == null ? "NULL" : "ok")} passable={cellData?.Passable} isStunned={m_Combatant.IsStunned} contained={cellData?.ContainedObject}");
        
        if (cellData == null || !cellData.Passable) return;

        if (m_Combatant.IsStunned)
        {
            GameManager.Instance.TurnManager.Tick();
            
            return;
        }

        GameManager.Instance.TurnManager.Tick();

        if (cellData.ContainedObject == null)
            MoveTo(target);
        else if (cellData.ContainedObject is ICombatant enemy)
            HandleEnemyDamage(enemy, target, cellData);
        else if (cellData.ContainedObject.PlayerWantsToEnter())
        {
            MoveTo(target);
            cellData.ContainedObject.PlayerEntered(m_Combatant);
        }
    }

    void TurnHappened() => m_Combatant.ChangeStamina(-1);

    public void Init() => m_IsGameOver = false;

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
