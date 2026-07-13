using System.Collections.Generic;
using UnityEngine;

public class CombatantStatusEffectsHUD : MonoBehaviour
{
    [SerializeField] private StatusEffectSlot m_SlotPrefab;

    private ICombatant m_Combatant;
    private readonly Dictionary<StatusEffectType, StatusEffectSlot> m_Slots = new();

    void OnDestroy()
    {
        if (m_Combatant != null)
        {
            m_Combatant.StatusApplied -= OnStatusApplied;
            m_Combatant.StatusRemoved -= OnStatusRemoved;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.TurnManager.OnTick -= RefreshAllSlots;
    }

    void OnStatusApplied(StatusEffect effect)
    {
        if (m_Slots.ContainsKey(effect.Type))
            return;

        StatusEffectSlot slot = Instantiate(m_SlotPrefab, transform);
        slot.Bind(effect);
        
        m_Slots[effect.Type] = slot;
    }

    void OnStatusRemoved(StatusEffect effect)
    {
        if (!m_Slots.TryGetValue(effect.Type, out StatusEffectSlot slot))
            return;

        Destroy(slot.gameObject);

        m_Slots.Remove(effect.Type);
    }

    void RefreshAllSlots()
    {
        foreach (StatusEffectSlot slot in m_Slots.Values) slot.Refresh();
    }

    public void Bind(ICombatant combatant)
    {
        m_Combatant = combatant;

        m_Combatant.StatusApplied += OnStatusApplied;
        m_Combatant.StatusRemoved += OnStatusRemoved;
        GameManager.Instance.TurnManager.OnTick += RefreshAllSlots;
    }
}
