using System;
using Board;
using Combat;
using Core;
using Player;
using UnityEngine;

namespace Enemy
{
    public abstract class EnemyController : MonoBehaviour
    {
        // Private references
        private BoardManager m_BoardManager;
        private TurnManager m_TurnManager;
        private PlayerController m_PlayerController;
        private Combatant m_Combatant;
        private CombatantAnimator m_CombatantAnimator;

        // Protected properties
        protected CombatantAnimator Animator => m_CombatantAnimator;
        protected Vector2Int PlayerCell => m_PlayerController.Combatant.Cell;

        // Public properties
        public Combatant Combatant => m_Combatant;

        void Awake()
        {
            m_Combatant = GetComponent<Combatant>();
            m_CombatantAnimator = GetComponent<CombatantAnimator>();

            m_TurnManager = GameManager.Instance.TurnManager;
            m_BoardManager = GameManager.Instance.BoardManager;
            m_PlayerController = GameManager.Instance.PlayerController;

            m_TurnManager.OnTick += OnTurnHappened;
        }

        void OnDestroy()
        {
            if (m_TurnManager != null)
                m_TurnManager.OnTick -= OnTurnHappened;
        }

        protected bool IsInLineOfSightToPlayer() =>
            m_BoardManager.IsInLineOfSight(m_Combatant.Cell, m_PlayerController.Combatant.Cell);

        protected bool IsAdjacentToPlayer(Vector2Int delta) =>
            Mathf.Abs(delta.x) + Mathf.Abs(delta.y) == 1;

        protected void MoveTowards(Vector2Int delta)
        {
            Vector2Int xDirection = delta.x > 0 ? Vector2Int.right : Vector2Int.left;
            Vector2Int yDirection = delta.y > 0 ? Vector2Int.up : Vector2Int.down;

            bool prioritizeX = Mathf.Abs(delta.x) > Mathf.Abs(delta.y);

            if (prioritizeX)
            {
                if (!m_Combatant.TryMoveTo(m_Combatant.Cell + xDirection))
                    m_Combatant.TryMoveTo(m_Combatant.Cell + yDirection);
            }
            else
            {
                if (!m_Combatant.TryMoveTo(m_Combatant.Cell + yDirection))
                    m_Combatant.TryMoveTo(m_Combatant.Cell + xDirection);
            }
        }

        protected Vector3 PlayerCellToWorld() => m_BoardManager.CellToWorld(PlayerCell);

        protected Vector2Int ComputeDeltaToPlayer() =>
            m_PlayerController.Combatant.Cell - m_Combatant.Cell;

        protected void DealDamageToPlayer()
        {
            m_Combatant.DealDamageTo(m_PlayerController.Combatant);
        }

        protected void AttackPlayer(Vector2Int direction)
        {
            m_Combatant.AttackTarget(
                m_PlayerController.Combatant,
                new Vector2Int(Math.Sign(direction.x), Math.Sign(direction.y))
            );
        }

        protected void ChaseOrAttack(Vector2Int delta)
        {
            m_CombatantAnimator.SetSpriteFacing(delta);

            if (IsAdjacentToPlayer(delta))
                AttackPlayer(delta);
            else
                MoveTowards(delta);
        }

        void OnTurnHappened()
        {
            if (m_Combatant.Stats.HP <= 0)
                return;

            if (m_Combatant.Statuses.IsStunned)
                return;

            ResolveEnemyAction();
        }

        protected abstract void ResolveEnemyAction();
    }
}
