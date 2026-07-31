using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Combatant : MonoBehaviour, ICombatant, ICellOccupant
{
    // Authored initial values
    [SerializeField] private int m_InitialMaxHP;
    [SerializeField] private int m_InitialMaxBlock;
    [SerializeField] private int m_InitialAttack;
    [SerializeField] private int m_InitialMaxStamina;

    // Status rolls
    [SerializeField] private StatusEffectRoll[] m_StatusRolls;

    // Private references
    private TurnManager m_TurnManager;
    private BoardManager m_BoardManager;

    // Runtime state
    private CombatantStats m_Stats = new();
    private readonly List<StatusEffect> m_StatusEffects = new();

    // Stats
    public int MaxHP => m_Stats.MaxHP;
    public int HP => m_Stats.HP;
    public int MaxBlock => m_Stats.MaxBlock;
    public int Block => m_Stats.Block;
    public int Attack => m_Stats.Attack;
    public int Stamina => m_Stats.Stamina;
    public int MaxStamina => m_Stats.MaxStamina;

    // Status effects
    public IReadOnlyList<StatusEffect> StatusEffects => m_StatusEffects;

    // Flags
    public bool IsStunned => m_StatusEffects.Any(e => e.Type == StatusEffectType.Stunned);

    GameObject ICellOccupant.GameObject => gameObject;

    // Events
    public event Action Defeated;
    public event Action Depleted;
    public event Action<DamageResult> Damaged;
    public event Action<Vector2Int> AttackPerformed;
    public event Action<int> HealthAdded;
    public event Action<int> BlockAdded;
    public event Action<int> StaminaChanged;
    public event Action<StatusEffect> StatusApplied;
    public event Action<StatusEffect> StatusRemoved;

    void Awake()
    {
        ResetState();

        m_TurnManager = GameManager.Instance.TurnManager;
        m_BoardManager = GameManager.Instance.BoardManager;

        GetComponent<CombatantAnimator>().Bind(this, m_TurnManager, m_BoardManager);
    }

    void Start() => m_TurnManager.OnTick += TickStatusEffects;

    void OnDestroy()
    {
        if (m_TurnManager != null)
            m_TurnManager.OnTick -= TickStatusEffects;
    }

    public DamageResult AttackTarget(ICombatant target, Vector2Int direction)
    {
        AttackPerformed?.Invoke(direction);

        return DealDamageTo(target);
    }

    public DamageResult DealDamageTo(ICombatant target) => CombatantDamage.ApplyDamage(this, target);

    public DamageResult TakeDamage(int amount)
    {
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

    public void DecrementStamina() => ChangeStamina(-1);

        public void TryApplyStatus(ICombatant target)
    {
        int level = GameManager.Instance.LevelManager.CurrentLevel;

        foreach (StatusEffectRoll roll in m_StatusRolls)
        {
            float probability = Mathf.Clamp01(roll.BaseProbability + roll.PerLevelBonus * level);
            if (UnityEngine.Random.value < probability)
            {
                target.ApplyStatusEffect(StatusEffectFactory.FromType(roll.Type, roll.Duration));

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
    
    public void IncreaseMaxHP(int amount) => m_Stats.MaxHP += amount;
    public void IncreaseMaxBlock(int amount) => m_Stats.MaxBlock += amount;
    public void IncreaseAttack(int amount) => m_Stats.Attack += amount;
    public void IncreaseMaxStamina(int amount) => m_Stats.MaxStamina += amount;

    public void UpgradeStat(CombatantStat stat, int amount)
    {
        switch (stat)
        {
            case CombatantStat.MaxHP: IncreaseMaxHP(amount); break;
            case CombatantStat.MaxBlock: IncreaseMaxBlock(amount); break;
            case CombatantStat.Attack: IncreaseAttack(amount); break;
            case CombatantStat.MaxStamina: IncreaseMaxStamina(amount); break;
        }
    }

    public void ApplyStatMultiplier(float multiplier)
    {
        m_Stats.MaxHP = Mathf.RoundToInt(m_InitialMaxHP * multiplier);
        m_Stats.MaxBlock = Mathf.RoundToInt(m_InitialMaxBlock * multiplier);
        m_Stats.Attack = Mathf.RoundToInt(m_InitialAttack * multiplier);
        m_Stats.HP = m_Stats.MaxHP;
        m_Stats.Block = 0;
    }

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
