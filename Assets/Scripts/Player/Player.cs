using System;
using System.Collections.Generic;
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
    public event Action Depleted;

    void Awake()
    {
        m_State = new CombatantState(m_MaxHP, m_Attack);
        m_State.AddBlock(7);

        m_Stamina = m_MaxStamina;
    }

    public DamageResult TakeDamage(int amount) => m_State.TakeDamage(amount);
    public void Heal(int amount) => m_State.Heal(amount);
    public void AddBlock(int amount) => m_State.AddBlock(amount);
    public void ApplyStatusEffect(StatusEffect effect) => m_State.ApplyStatusEffect(effect);
    public void ChangeStamina(int amount) {
        int previous = m_Stamina;

        m_Stamina = Mathf.Clamp(m_Stamina + amount, 0, m_MaxStamina);
        if (previous > 0 && m_Stamina == 0)
        {
            Depleted?.Invoke();
        }
    }
}
