using System.Collections.Generic;
using UnityEngine;

public abstract class Combatant : MonoBehaviour
{
    [SerializeField]
    private int m_MaxHP = 10;
    private List<StatusEffect> m_StatusEffects;

    public int MaxHP => m_MaxHP;
    public int CurrentHP { get; private set; }
    public int CurrentBlock { get; private set; }
    public IReadOnlyList<StatusEffect> StatusEffects => m_StatusEffects;

    protected virtual void Awake()
    {
        CurrentHP = m_MaxHP;
        CurrentBlock = 0;
        m_StatusEffects = new List<StatusEffect>();
    }

    public void Heal(int amount) => CurrentHP = Mathf.Clamp(CurrentHP + amount, 0, m_MaxHP);
    
    public void TakeDamage(int amount)
    {
        int damageAfterBlock = Mathf.Max(amount - CurrentBlock, 0);
        CurrentBlock = Mathf.Max(CurrentBlock - amount, 0);
        CurrentHP = Mathf.Max(CurrentHP - damageAfterBlock, 0);
    }

    public void AddBlock(int amount) => CurrentBlock = Mathf.Max(CurrentBlock + amount, 0);

    public void AddStatusEffect(StatusEffect effect) => m_StatusEffects.Add(effect);
}
