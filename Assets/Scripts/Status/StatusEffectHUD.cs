using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHUD : MonoBehaviour
{
    [SerializeField] private Player m_Player;
    [SerializeField] private StatusEffectSlot m_SlotPrefab;

    private readonly Dictionary<StatusEffectType, StatusEffectSlot> m_Slots = new();

    void Awake()
    {
        m_Player.StatusApplied += OnStatusApplied;
        m_Player.StatusRemoved += OnStatusRemoved;
    }

    void Start() => GameManager.Instance.TurnManager.OnTick += RefreshAllSlots;

    void OnDestroy()
    {
        if (m_Player != null)
        {
            m_Player.StatusApplied -= OnStatusApplied;
            m_Player.StatusRemoved -= OnStatusRemoved;
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
}
