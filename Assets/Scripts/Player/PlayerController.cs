using Board;
using Combat;
using Core;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        // References
        private BoardManager m_BoardManager;
        private TurnManager m_TurnManager;
        private PlayerInputActions m_InputActions;
        private Combatant m_Combatant;
        private CombatantAnimator m_CombatantAnimator;
        private SpriteRenderer m_SpriteRenderer;

        private bool m_IsGameOver;

        public Combatant Combatant => m_Combatant;

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
            m_BoardManager = GameManager.Instance.BoardManager;
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

        void ResolvePlayerAction(Vector2Int target)
        {
            if (m_Combatant.Statuses.IsStunned)
            {
                m_TurnManager.BeginTurn();

                return;
            }

            ICellOccupant occupant = m_BoardManager.GetOccupantAt(target);

            if (occupant == null)
                m_Combatant.TryMoveTo(target);
            else if (occupant is Combatant enemy)
                m_Combatant.AttackTarget(enemy, target - m_Combatant.Cell);
            else if (occupant is CellObject obj && obj.PlayerWantsToEnter())
            {
                obj.PlayerEntered(this);
                m_Combatant.TryMoveTo(target);
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

            Vector2Int target = m_Combatant.Cell + direction;

            BoardManager.CellData cell = m_BoardManager.GetCellData(target);
            if (cell == null || !cell.Passable)
                return;

            ResolvePlayerAction(target);
        }

        void TurnHappened() => m_Combatant.Stats.ChangeStamina(-1);

        public void Init() => m_IsGameOver = false;

        public void GameOver() => m_IsGameOver = true;

        public void Spawn(Vector2Int cell) => m_Combatant.Teleport(cell);
    }
}
