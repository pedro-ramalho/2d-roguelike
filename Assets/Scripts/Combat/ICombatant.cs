using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public readonly struct DamageResult
{
    public int BlockLost { get; }
    public int HPLost { get; }

    public DamageResult(int blockLost, int hpLost)
    {
        BlockLost = blockLost;
        HPLost = hpLost;
    }
}

public interface ICombatant
{
    // Stats
    int Attack { get; }
    int MaxHP { get; }
    int HP { get; }
    int Block { get; }

    // Status Effects
    IReadOnlyList<StatusEffect> StatusEffects { get; }

    // States
    bool IsStunned { get; }

    // Events
    public event Action<DamageResult> Damaged;
    public event Action<int> HealthAdded;
    public event Action<int> BlockAdded;

    DamageResult TakeDamage(int amount);
    void Heal(int amount);
    void AddBlock(int amount);
    void ApplyStatusEffect(StatusEffect effect);
}
