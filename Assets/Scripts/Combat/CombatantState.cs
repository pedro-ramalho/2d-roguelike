using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatantState
{
    private readonly List<StatusEffect> m_StatusEffects = new();
    
    // Stats
    public int Attack { get; }
    public int MaxHP { get; }
    public int HP { get; private set; }
    public int Block { get; private set; }
    public IReadOnlyList<StatusEffect> StatusEffects => m_StatusEffects;

    // Flags
    public bool IsStunned => m_StatusEffects.Any(e => e.Type == StatusEffectType.Stunned);

    public CombatantState(int maxHP, int attack)
    {
        MaxHP = maxHP;
        HP = maxHP;
        Attack = attack;
    }

    public List<StatusEffect> TickStatusEffects(ICombatant owner)
    {
        List<StatusEffect> removed = new List<StatusEffect>();
        
        foreach (StatusEffect effect in m_StatusEffects) effect.OnTurnEnd(owner);

        for (int i = m_StatusEffects.Count - 1; i >= 0; i--)
        {
            StatusEffect effect = m_StatusEffects[i];

            if (effect.IsDepleted)
            {
                removed.Add(effect);
                effect.OnRemoved(owner);
                m_StatusEffects.RemoveAt(i);
            }
        }
        
        return removed;
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

    public void ApplyStatusEffect(StatusEffect effect, ICombatant owner) 
    {
        foreach (StatusEffect sf in m_StatusEffects)
        {
            if (sf.Type == effect.Type)
            {
                sf.Duration = Mathf.Max(sf.Duration, effect.Duration);
                
                return;
            }    
        }
        
        m_StatusEffects.Add(effect); 
        effect.OnApplied(owner);
    }
}
