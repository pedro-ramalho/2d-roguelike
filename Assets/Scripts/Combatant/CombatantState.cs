using System.Collections.Generic;
using UnityEngine;

public class CombatantState
{
    private readonly List<StatusEffect> m_StatusEffects = new();
    
    public int Attack { get; }
    public int MaxHP { get; }
    public int HP { get; private set; }
    public int Block { get; private set; }
    public IReadOnlyList<StatusEffect> StatusEffects => m_StatusEffects;

    public CombatantState(int maxHP, int attack)
    {
        MaxHP = maxHP;
        HP = maxHP;
        Attack = attack;
    }

    public DamageResult TakeDamage(int amount)
    {
        int blockLost = Mathf.Min(Block, amount);
        Block -= blockLost;

        int hpLost = Mathf.Max(0, amount - blockLost);
        HP -= hpLost;

        return new DamageResult(blockLost, hpLost);
    }

    public void Heal(int amount) => HP = Mathf.Clamp(HP + amount, 0, MaxHP);
    
    public void AddBlock(int amount) => Block = Mathf.Max(Block + amount, 0);

    public void ApplyStatusEffect(StatusEffect effect) =>m_StatusEffects.Add(effect);
}
