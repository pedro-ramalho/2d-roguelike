using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // References
    private BoardManager m_Board;
    private PlayerInputActions m_InputActions;
    private PlayerStats m_PlayerStats;
    private Combatant m_Combatant;
    private CombatantAnimator m_CombatantAnimator;
    private SpriteRenderer m_SpriteRenderer;

    // State
    private Vector2Int m_CellPosition;
    private bool m_IsGameOver;
    private bool m_IsProcessingTurn;
    
    public Combatant Combatant => m_Combatant;
    public PlayerStats PlayerStats => m_PlayerStats;
    public Vector2Int Cell => m_CellPosition;

    void Start() => GameManager.Instance.TurnManager.OnTick += TurnHappened;
    
    void Awake()
    {
        m_InputActions = new PlayerInputActions();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        m_Combatant = GetComponent<Combatant>();
        m_CombatantAnimator = GetComponent<CombatantAnimator>();

        m_PlayerStats = GetComponent<PlayerStats>();
    }

    void OnEnable() => m_InputActions.Player.Enable();
    
    void OnDisable()
    {
        m_InputActions.Player.Disable();
        m_IsProcessingTurn = false;  
    } 
    
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
        m_Combatant.AttackTarget(enemy, target - m_CellPosition);

        if (enemy.HP <= 0)
        {            
            cell.ContainedObject = null;
            
            MoveTo(target, false);
        }
    }

    void HandleMovementInput()
    {
        if (m_IsProcessingTurn) return;

        Vector2Int direction = GetInputDirection();
        if (direction == Vector2Int.zero) return;

        Vector2Int target = m_CellPosition + direction;

        BoardManager.CellData cellData = m_Board.GetCellData(target);
        if (cellData == null || !cellData.Passable) return;

        StartCoroutine(ProcessTurn(target, cellData));
    }

    IEnumerator ProcessTurn(Vector2Int target, BoardManager.CellData cellData)
    {
        m_IsProcessingTurn = true;

        if (!m_Combatant.IsStunned)
        {
            if (cellData.ContainedObject == null)
                MoveTo(target, false);
            else if (cellData.ContainedObject is ICombatant enemy)
                HandleEnemyDamage(enemy, target, cellData);
            else if (cellData.ContainedObject is CellObject obj && obj.PlayerWantsToEnter())
            {
                MoveTo(target, false);
                obj.PlayerEntered(this);
            }

            yield return new WaitForSeconds(m_CombatantAnimator.WalkDuration);
        }

        GameManager.Instance.TurnManager.Tick();

        yield return new WaitForSeconds(m_CombatantAnimator.WalkDuration);

        m_IsProcessingTurn = false;
    }

    void TurnHappened() => m_PlayerStats.DecrementStamina();

    public void Init() => m_IsGameOver = false;

    public void GameOver() => m_IsGameOver = true;

    public void SetVisible(bool visible) => m_SpriteRenderer.enabled = visible;

    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_Board = boardManager;
        MoveTo(cell, true);
    }

    public void MoveTo(Vector2Int cell, bool snap)
    {
        m_CellPosition = cell;
        
        if (snap)
            transform.position = m_Board.CellToWorld(cell);
        else
            m_CombatantAnimator.PlayWalkAnimation(cell);
    }
}
