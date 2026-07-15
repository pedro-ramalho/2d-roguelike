using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour, ICombatant
{
    [SerializeField] private int m_MaxHP = 100;
    [SerializeField] private int m_MaxStamina = 20;
    [SerializeField] private int m_Attack = 10;
    private int m_Stamina;
    private CombatantState m_State;

    public int Attack => m_State.Attack;
    public int MaxHP => m_MaxHP;
    public int HP => m_State.HP;
    public int MaxStamina => m_MaxStamina;
    public int Stamina => m_Stamina;
    public int Block => m_State.Block;
    public IReadOnlyList<StatusEffect> StatusEffects => m_State.StatusEffects;

    public bool IsStunned => m_State.IsStunned;
    
    // Events
    public event Action Depleted;
    public event Action Defeated;
    public event Action<StatusEffect> StatusApplied;
    public event Action<StatusEffect> StatusRemoved;
    public event Action<DamageResult> Damaged;
    public event Action<int> HealthAdded;
    public event Action<int> BlockAdded;
    public event Action<int> StaminaChanged;

    void Awake()
    {
        Init();
    
        GameManager.Instance.TurnManager.OnTick += TickStatusEffects;

        GetComponent<CombatantAnimator>().Bind(this);
        GetComponentInChildren<CombatantBarsHUD>().Bind(this);
        GetComponentInChildren<CombatantStatusEffectsHUD>().Bind(this);
        GetComponentInChildren<CombatantFloatersHUD>().Bind(this);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.TurnManager.OnTick -= TickStatusEffects;
    }

    void TickStatusEffects()
    {
        List<StatusEffect> removed = m_State.TickStatusEffects(this);

        foreach (StatusEffect effect in removed) StatusRemoved?.Invoke(effect);
    } 

    void Init()
    {
        m_State = new CombatantState(m_MaxHP, m_Attack);
        m_Stamina = m_MaxStamina;        
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

    public void ChangeStamina(int amount) {
        int previous = m_Stamina;

        m_Stamina = Mathf.Clamp(m_Stamina + amount, 0, m_MaxStamina);

        StaminaChanged?.Invoke(m_Stamina);
        
        if (previous > 0 && m_Stamina == 0)
            Depleted?.Invoke();
    }

    public void ResetState() => Init();
}
