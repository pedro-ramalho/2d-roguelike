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
        // -- Combatant Stats --
        [Header("Initial Stats")]
        // Authored initial values
        [SerializeField]
        private int m_InitialMaxHP; // STAT

        [SerializeField]
        private int m_InitialMaxBlock; // STAT

        [SerializeField]
        private int m_InitialAttack; // STAT

        [SerializeField]
        private int m_InitialMaxStamina; // STAT

        // -- Combatant Band-Specific Stats --
        [Header("Band Stats")]
        [SerializeField]
        private BandStats[] m_BandStats; // BAND-SPECIFIC STATS

        // -- Combatant Status Effects --
        [Header("Status Effects")]
        [SerializeField]
        private StatusEffectRoll[] m_StatusRolls; // STATUS EFFECT ROLL TABLE

        // -- Private References --
        private TurnManager m_TurnManager; // TURN MANAGER REFERENCE
        private BoardManager m_BoardManager; // BOARD MANAGER REFERENCE

        // -- Runtime State --
        private CombatantStats m_Stats = new(); // RUNTIME COMBATANT STATS
        private readonly List<StatusEffect> m_StatusEffects = new(); // RUNTIME COMBATANT STATUS EFFECT LIST
        private Vector2Int m_Cell; // RUNTIME COMBATANT CELL POSITION

        // Stats
        public int MaxHP => m_Stats.MaxHP;
        public int HP => m_Stats.HP;
        public int MaxBlock => m_Stats.MaxBlock;
        public int Block => m_Stats.Block;
        public int Attack => m_Stats.Attack;
        public int Stamina => m_Stats.Stamina;
        public int MaxStamina => m_Stats.MaxStamina;

        public Vector2Int Cell => m_Cell;

        // Status effects
        public IReadOnlyList<StatusEffect> StatusEffects => m_StatusEffects;

        // -- Combatant Flags --
        public bool IsStunned => m_StatusEffects.Any(e => e.Type == StatusEffectType.Stunned); // RUNTIME FLAG
        public bool IsGodMode { get; set; } // RUNTIME FLAG

        GameObject ICellOccupant.GameObject => gameObject;

        // -- Combatant Events --
        public event Action Defeated; // DEATH EVENT
        public event Action Depleted; // DEATH EVENT
        public event Action<DamageResult> Damaged; // DAMAGE TAKEN EVENT
        public event Action<Vector2Int> AttackPerformed; // ATTACK EVENT
        public event Action<int> HealthAdded; // STAT-MODIFICATION EVENT
        public event Action<int> BlockAdded; // STAT-MODIFICATION EVENT
        public event Action<int> StaminaChanged; // STAT-MODIFICATION EVENT
        public event Action<StatusEffect> StatusApplied; // STATUS EFFECT EVENT
        public event Action<StatusEffect> StatusRemoved; // STATUS EFFECT EVENT
        public event Action StateReset; // STATE RESET EVENT

        // -- SETUP --
        void Awake()
        {
            ResetState();

            m_TurnManager = GameManager.Instance.TurnManager;
            m_BoardManager = GameManager.Instance.BoardManager;

            m_Cell = m_BoardManager.WorldToCell(transform.position);

            GetComponent<CombatantAnimator>().Bind(this, m_TurnManager, m_BoardManager);
        }

        void Start() => m_TurnManager.OnTick += TickStatusEffects;

        void OnDestroy()
        {
            if (m_TurnManager != null)
                m_TurnManager.OnTick -= TickStatusEffects;
        }

        // -- CELL POSITION --
        public void SetCell(Vector2Int cell) => m_Cell = cell;

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
            if (IsGodMode)
                return new DamageResult(0, 0);

            int previousHP = m_Stats.HP;

            int blockLost = Mathf.Min(m_Stats.Block, amount);
            m_Stats.Block -= blockLost;

            int hpLost = Mathf.Max(0, amount - blockLost);
            m_Stats.HP -= hpLost;

            DamageResult result = new DamageResult(blockLost, hpLost);

            if (previousHP > 0 && m_Stats.HP <= 0)
                Defeated?.Invoke();

            Damaged?.Invoke(result);

            return result;
        }

        // -- COMBATANT STAT MODIFICATION --
        public void Heal(int amount)
        {
            m_Stats.HP = Mathf.Clamp(m_Stats.HP + amount, 0, m_Stats.MaxHP);
            HealthAdded?.Invoke(amount);
        }

        public void AddBlock(int amount)
        {
            m_Stats.Block = Mathf.Clamp(m_Stats.Block + amount, 0, m_Stats.MaxBlock);
            BlockAdded?.Invoke(amount);
        }

        public void ChangeStamina(int amount)
        {
            int previous = m_Stats.Stamina;

            m_Stats.Stamina = Mathf.Clamp(m_Stats.Stamina + amount, 0, m_Stats.MaxStamina);
            StaminaChanged?.Invoke(m_Stats.Stamina);

            if (previous > 0 && m_Stats.Stamina == 0)
                Depleted?.Invoke();
        }

        public void UpgradeStat(CombatantStat stat, int amount)
        {
            switch (stat)
            {
                case CombatantStat.MaxHP:
                    m_Stats.MaxHP += amount;
                    break;
                case CombatantStat.MaxBlock:
                    m_Stats.MaxBlock += amount;
                    break;
                case CombatantStat.Attack:
                    m_Stats.Attack += amount;
                    break;
                case CombatantStat.MaxStamina:
                    m_Stats.MaxStamina += amount;
                    break;
            }
        }

        public void ApplyStatMultiplier(float hpMult, float blockMult, float attackMult)
        {
            m_Stats.MaxHP = Mathf.RoundToInt(m_Stats.MaxHP * hpMult);
            m_Stats.MaxBlock = Mathf.RoundToInt(m_Stats.MaxBlock * blockMult);
            m_Stats.Attack = Mathf.RoundToInt(m_Stats.Attack * attackMult);
            m_Stats.HP = m_Stats.MaxHP;
            m_Stats.Block = 0;
        }

        public void RefreshStats()
        {
            m_Stats.HP = MaxHP;
            m_Stats.Stamina = MaxStamina;
        }

        public void ApplyBandStats()
        {
            BandType current = GameManager.Instance.LevelManager.CurrentBand.Type;

            foreach (BandStats entry in m_BandStats)
            {
                if (entry.Band == current)
                {
                    m_Stats.MaxHP = entry.MaxHP;
                    m_Stats.HP = entry.MaxHP;
                    m_Stats.Attack = entry.Attack;
                    m_Stats.MaxBlock = entry.MaxBlock;

                    return;
                }
            }
        }

        // -- COMBATANT STATUS EFFECT MANAGEMENT --
        public void RollStatusOnHit(Combatant target)
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

        // -- COMBATANT STATE RESET --
        public void ResetState()
        {
            m_Stats = new CombatantStats
            {
                MaxHP = m_InitialMaxHP,
                HP = m_InitialMaxHP,
                MaxBlock = m_InitialMaxBlock,
                Block = 0,
                Attack = m_InitialAttack,
                MaxStamina = m_InitialMaxStamina,
                Stamina = m_InitialMaxStamina,
            };

            m_StatusEffects.Clear();

            StateReset?.Invoke();
        }
    }
}
