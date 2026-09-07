using System;
using Board;
using Core;
using UnityEngine;

namespace Combat
{
    public class Combatant : MonoBehaviour, ICellOccupant
    {
        private TurnManager m_TurnManager;
        private BoardManager m_BoardManager;

        private CombatantStats m_Stats;
        private CombatantStatusEffects m_Statuses;
        private Vector2Int m_Cell;

        public Vector2Int Cell => m_Cell;

        GameObject ICellOccupant.GameObject => gameObject;

        public CombatantStats Stats => m_Stats;
        public CombatantStatusEffects Statuses => m_Statuses;

        public event Action Defeated;
        public event Action<Vector2Int> AttackPerformed;

        void Awake()
        {
            m_TurnManager = GameManager.Instance.TurnManager;
            m_BoardManager = GameManager.Instance.BoardManager;

            m_Stats = GetComponent<CombatantStats>();
            m_Statuses = GetComponent<CombatantStatusEffects>();

            m_Cell = m_BoardManager.WorldToCell(transform.position);

            GetComponent<CombatantAnimator>().Bind(this, m_TurnManager, m_BoardManager);
        }

        void OnDestroy()
        {
            if (m_BoardManager != null)
                RemoveSelfFromCell();
        }

        void RemoveSelfFromCell()
        {
            BoardManager.CellData data = m_BoardManager.GetCellData(m_Cell);
            if (data != null && ReferenceEquals(data.ContainedObject, this))
                data.ContainedObject = null;
        }

        public void Teleport(Vector2Int cell)
        {
            RemoveSelfFromCell();

            m_Cell = cell;
            transform.position = m_BoardManager.CellToWorld(cell);

            BoardManager.CellData data = m_BoardManager.GetCellData(cell);
            if (data != null)
                data.ContainedObject = this;
        }

        public bool CanMoveTo(Vector2Int cell)
        {
            BoardManager.CellData data = m_BoardManager.GetCellData(cell);
            return data != null && data.Passable && data.ContainedObject == null;
        }

        public bool TryMoveTo(Vector2Int cell)
        {
            if (!CanMoveTo(cell))
                return false;

            Vector2Int direction = cell - m_Cell;

            RemoveSelfFromCell();

            BoardManager.CellData targetData = m_BoardManager.GetCellData(cell);
            targetData.ContainedObject = this;

            m_Cell = cell;

            GetComponent<CombatantAnimator>().PlayWalkAnimation(cell, direction);
            return true;
        }

        public DamageResult AttackTarget(Combatant target, Vector2Int direction)
        {
            AttackPerformed?.Invoke(direction);

            return DealDamageTo(target);
        }

        public DamageResult DealDamageTo(Combatant target) =>
            CombatantDamage.ApplyDamage(this, target);

        public DamageResult TakeDamage(int amount)
        {
            int previousHP = m_Stats.HP;

            DamageResult result = m_Stats.TakeDamage(amount);
            if (previousHP > 0 && m_Stats.HP <= 0)
            {
                RemoveSelfFromCell();
                Defeated?.Invoke();
            }

            return result;
        }
    }
}
