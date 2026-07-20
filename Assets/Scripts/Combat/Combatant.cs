using System;
using System.Collections.Generic;
using UnityEngine;

public class Combatant : MonoBehaviour, ICombatant, ICellOccupant
{
    // Shared stats
    [SerializeField] private int m_MaxHP;
    [SerializeField] private int m_Attack;

    // Private references
    private TurnManager m_TurnManager;
    private BoardManager m_BoardManager;

    // State
    private CombatantState m_State;

    // Stat properties
    public int Attack => m_State.Attack;
    public int MaxHP => m_State.MaxHP;
    public int HP => m_State.HP;
    public int Block => m_State.Block;

    // Status effects
    public IReadOnlyList<StatusEffect> StatusEffects => m_State.StatusEffects;

    // Shared
    public bool IsStunned => m_State.IsStunned;

    GameObject ICellOccupant.GameObject => gameObject;

    // Shared events
    public event Action Defeated;
    public event Action<DamageResult> Damaged;
    public event Action<Vector2Int> AttackPerformed;
    public event Action<int> HealthAdded;
    public event Action<int> BlockAdded;
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

    void TickStatusEffects()
    {
        List<StatusEffect> removed = m_State.TickStatusEffects(this);

        foreach (StatusEffect effect in removed)
            StatusRemoved?.Invoke(effect);
    }

    public void AddBlock(int amount)
    {
        m_State.AddBlock(amount);
        BlockAdded?.Invoke(amount);
    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        m_State.ApplyStatusEffect(effect, this);
        StatusApplied?.Invoke(effect);
    }

    public void AttackTarget(ICombatant target, Vector2Int direction)
    {
        AttackPerformed?.Invoke(direction);
        CombatantDamage.ApplyDamage(this, target);
    }

    public void Heal(int amount)
    {
        m_State.Heal(amount);
        HealthAdded?.Invoke(amount);
    }

    public DamageResult TakeDamage(int amount)
    {
        int previousHP = m_State.HP;

        DamageResult result = m_State.TakeDamage(amount);

        if (previousHP > 0 && m_State.HP <= 0)
            Defeated?.Invoke();

        Damaged?.Invoke(result);

        return result;
    }

    public void ResetState() => m_State = new CombatantState(m_MaxHP, m_Attack);
}
