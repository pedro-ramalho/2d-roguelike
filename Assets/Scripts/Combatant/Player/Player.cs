using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICombatant
{
    [SerializeField] 
    private int m_MaxHP = 100;
    private CombatantState m_State;

    public int MaxHP => m_MaxHP;
    public int HP => m_State.HP;
    public int Block => m_State.Block;
    public IReadOnlyList<StatusEffect> StatusEffects => m_State.StatusEffects;

    void Awake()
    {
        m_State = new CombatantState(m_MaxHP);
    }

    public DamageResult TakeDamage(int amount) => m_State.TakeDamage(amount);
    public void Heal(int amount) => m_State.Heal(amount);
    public void AddBlock(int amount) => m_State.AddBlock(amount);
    public void ApplyStatusEffect(StatusEffect effect) => m_State.ApplyStatusEffect(effect);
}
