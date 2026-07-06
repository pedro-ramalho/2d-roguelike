using System.Collections.Generic;
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

public class ICombatant
{
    int MaxHP { get; }
    int HP { get; }
    int Block { get; }
    IReadOnlyList<StatusEffect> StatusEffects { get; }

    DamageResult TakeDamage(int amount);
    void Heal(int amount);
    void AddBlock(int amount);
    void ApplyStatusEffect(StatusEffect effect);
}
