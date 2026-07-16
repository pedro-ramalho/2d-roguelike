using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyObject : CellObject, ICombatant
{
    [SerializeField] private int m_MaxHP = 1000;
    [SerializeField] private int m_Attack = 2;
    private CombatantState m_State;

    public int Attack => m_State.Attack;
    public int MaxHP => m_State.MaxHP;
    public int HP => m_State.HP;
    public int Block => m_State.Block;

    public IReadOnlyList<StatusEffect> StatusEffects => m_State.StatusEffects;

    public bool IsStunned => m_State.IsStunned;

    // Events
    public event Action Defeated;
    public event Action<Vector2Int> AttackPerformed;
    public event Action<DamageResult> Damaged;
    public event Action<int> HealthAdded;
    public event Action<int> BlockAdded;
    public event Action<StatusEffect> StatusApplied;
    public event Action<StatusEffect> StatusRemoved;

    void Awake()
    {
        m_State = new CombatantState(m_MaxHP, m_Attack);

        GameManager.Instance.TurnManager.OnTick += TickStatusEffects;

        GetComponent<CombatantAnimator>().Bind(this);
        GetComponentInChildren<CombatantBarsHUD>().Bind(this);
        GetComponentInChildren<CombatantStatusEffectsHUD>().Bind(this);
        GetComponentInChildren<CombatantFloatersHUD>().Bind(this);
    }

    void OnDestroy() => GameManager.Instance.TurnManager.OnTick -= TickStatusEffects;

    void TickStatusEffects()
    {
        List<StatusEffect> removed = m_State.TickStatusEffects(this);

        foreach (StatusEffect effect in removed) StatusRemoved?.Invoke(effect);
    }

    public void AttackTarget(ICombatant target, Vector2Int direction)
    {
        
    }

    public DamageResult TakeDamage(int amount) 
    {
        DamageResult result = m_State.TakeDamage(amount);
        Damaged?.Invoke(result);

        return result;
    }

    public void Heal(int amount) 
    { 
        m_State.Heal(amount);
        HealthAdded?.Invoke(amount); 
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
}
