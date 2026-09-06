using System;
using System.Collections.Generic;
using System.Linq;
using Board;
using Core;
using Level;
using Status;
using UnityEngine;

namespace Combat
{
    public class Combatant : MonoBehaviour, ICellOccupant
    {
        // -- Combatant Status Effects --
        [Header("Status Effects")]
        [SerializeField]
        private StatusEffectRoll[] m_StatusRolls; // STATUS EFFECT ROLL TABLE

        // -- Private References --
        private TurnManager m_TurnManager; // TURN MANAGER REFERENCE
        private BoardManager m_BoardManager; // BOARD MANAGER REFERENCE

        // -- Runtime State --
        private CombatantStats m_Stats; // RUNTIME COMBATANT STATS
        private readonly List<StatusEffect> m_StatusEffects = new(); // RUNTIME COMBATANT STATUS EFFECT LIST
        private Vector2Int m_Cell; // RUNTIME COMBATANT CELL POSITION

        public Vector2Int Cell => m_Cell;

        // Status effects
        public IReadOnlyList<StatusEffect> StatusEffects => m_StatusEffects;

        // -- Combatant Flags --
        public bool IsStunned => m_StatusEffects.Any(e => e.Type == StatusEffectType.Stunned); // RUNTIME FLAG

        GameObject ICellOccupant.GameObject => gameObject;

        public CombatantStats Stats => m_Stats;

        // -- Combatant Events --
        public event Action Defeated; // DEATH EVENT
        public event Action<Vector2Int> AttackPerformed; // ATTACK EVENT
        public event Action<StatusEffect> StatusApplied; // STATUS EFFECT EVENT
        public event Action<StatusEffect> StatusRemoved; // STATUS EFFECT EVENT

        // -- SETUP --
        void Awake()
        {
            m_TurnManager = GameManager.Instance.TurnManager;
            m_BoardManager = GameManager.Instance.BoardManager;

            m_Stats = GetComponent<CombatantStats>();

            m_Cell = m_BoardManager.WorldToCell(transform.position);

            GetComponent<CombatantAnimator>().Bind(this, m_TurnManager, m_BoardManager);
        }

        void Start() => m_TurnManager.OnTick += TickStatusEffects;

        void OnDestroy()
        {
            if (m_TurnManager != null)
                m_TurnManager.OnTick -= TickStatusEffects;

            if (m_BoardManager != null)
                RemoveSelfFromCell();
        }

        // -- CELL POSITION --
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

        // -- COMBATANT DAMAGE --
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

        // -- COMBATANT STATUS EFFECT MANAGEMENT --
        internal void RollStatusOnHit(Combatant target)
        {
            int level = GameManager.Instance.LevelManager.CurrentLevel;

            foreach (StatusEffectRoll roll in m_StatusRolls)
            {
                float probability = Mathf.Clamp(
                    roll.BaseProbability + roll.PerLevelBonus * level,
                    0,
                    GameManager.Instance.ProgressionSettings.MaxStatusChance
                );
                if (UnityEngine.Random.value < probability)
                {
                    target.ApplyStatusEffect(
                        StatusEffectFactory.FromType(roll.Type, roll.Duration)
                    );

                    return;
                }
            }
        }

        public void ApplyStatusEffect(StatusEffect effect)
        {
            foreach (StatusEffect sf in m_StatusEffects)
            {
                if (sf.Type == effect.Type)
                {
                    sf.Duration = Mathf.Max(sf.Duration, effect.Duration);
                    StatusApplied?.Invoke(effect);

                    return;
                }
            }

            m_StatusEffects.Add(effect);
            effect.OnApplied(this);

            StatusApplied?.Invoke(effect);
        }

        public void RemoveStatusEffect(StatusEffect effect)
        {
            m_StatusEffects.Remove(effect);
            effect.OnRemoved(this);

            StatusRemoved?.Invoke(effect);
        }

        void TickStatusEffects()
        {
            foreach (StatusEffect effect in m_StatusEffects)
                effect.OnTurnEnd(this);

            for (int i = m_StatusEffects.Count - 1; i >= 0; i--)
            {
                StatusEffect effect = m_StatusEffects[i];

                if (effect.IsDepleted)
                {
                    m_StatusEffects.RemoveAt(i);
                    effect.OnRemoved(this);

                    StatusRemoved?.Invoke(effect);
                }
            }
        }
    }
}
