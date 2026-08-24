using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // References
    private BoardManager m_BoardManager;
    private TurnManager m_TurnManager;
    private PlayerInputActions m_InputActions;
    private Combatant m_Combatant;
    private CombatantAnimator m_CombatantAnimator;
    private SpriteRenderer m_SpriteRenderer;

    // State
    private Vector2Int m_CellPosition;
    private bool m_IsGameOver;

    public Combatant Combatant => m_Combatant;
    public Vector2Int Cell => m_CellPosition;

    void Start()
    {
        m_TurnManager = GameManager.Instance.TurnManager;
        m_TurnManager.OnTick += TurnHappened;
    }

    void Awake()
    {
        m_InputActions = new PlayerInputActions();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        m_Combatant = GetComponent<Combatant>();
        m_CombatantAnimator = GetComponent<CombatantAnimator>();
    }

    void OnEnable() => m_InputActions.Player.Enable();

    void OnDisable() => m_InputActions.Player.Disable();

    void OnDestroy()
    {
        m_InputActions.Dispose();

        if (m_TurnManager != null)
            m_TurnManager.OnTick -= TurnHappened;
    }

    void Update()
    {
        if (m_IsGameOver)
            return;

        HandleMovementInput();
    }

    Vector2Int GetInputDirection()
    {
        if (m_InputActions.Player.MoveUp.WasPressedThisFrame())
            return Vector2Int.up;
        if (m_InputActions.Player.MoveDown.WasPressedThisFrame())
            return Vector2Int.down;
        if (m_InputActions.Player.MoveLeft.WasPressedThisFrame())
            return Vector2Int.left;
        if (m_InputActions.Player.MoveRight.WasPressedThisFrame())
            return Vector2Int.right;

        return Vector2Int.zero;
    }

    void HandleEnemyDamage(ICombatant enemy, Vector2Int target, BoardManager.CellData cell)
    {
        m_Combatant.AttackTarget(enemy, target - m_CellPosition);

        if (enemy.HP <= 0 && cell.ContainedObject == (ICellOccupant)enemy)
            cell.ContainedObject = null;
    }

    void ResolvePlayerAction(BoardManager.CellData cell, Vector2Int target)
    {
        if (m_Combatant.IsStunned)
        {
            m_TurnManager.BeginTurn();

            return;
        }

        if (cell.ContainedObject == null)
            MoveTo(target, false);
        else if (cell.ContainedObject is ICombatant enemy)
            HandleEnemyDamage(enemy, target, cell);
        else if (cell.ContainedObject is CellObject obj && obj.PlayerWantsToEnter())
        {
            MoveTo(target, false);
            obj.PlayerEntered(this);
        }

        m_TurnManager.BeginTurn();
    }

    void HandleMovementInput()
    {
        if (m_TurnManager.IsProcessingTurn)
            return;

        Vector2Int direction = GetInputDirection();
        if (direction == Vector2Int.zero)
            return;

        m_CombatantAnimator.SetSpriteFacing(direction);

        Vector2Int target = m_CellPosition + direction;

        BoardManager.CellData cell = m_BoardManager.GetCellData(target);
        if (cell == null || !cell.Passable)
            return;

        ResolvePlayerAction(cell, target);
    }

    void TurnHappened() => m_Combatant.DecrementStamina();

    public void Init() => m_IsGameOver = false;

    public void GameOver() => m_IsGameOver = true;

    public void SetVisible(bool visible)
    {
        m_SpriteRenderer.enabled = visible;

        if (visible)
        {
            Color color = m_SpriteRenderer.color;
            color.a = 1f;
            m_SpriteRenderer.color = color;
        }
    }

    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_BoardManager = boardManager;
        MoveTo(cell, true);
    }

    public void MoveTo(Vector2Int cell, bool snap)
    {
        Vector2Int direction = cell - m_CellPosition;
        m_CellPosition = cell;

        if (snap)
            transform.position = m_BoardManager.CellToWorld(cell);
        else
            m_CombatantAnimator.PlayWalkAnimation(cell, direction);
    }
}
