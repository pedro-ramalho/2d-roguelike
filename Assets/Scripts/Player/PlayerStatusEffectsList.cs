using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerStatusEffectsList : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset m_SlotTemplate;

    private Player m_Player;
    private VisualElement m_Container;
    private readonly Dictionary<StatusEffectType, VisualElement> m_Slots = new();

    void Start()
    {
        m_Player = GetComponent<Player>();
        if (m_Player == null) return;

        UIDocument doc = GameManager.Instance.UIDoc;
        m_Container = doc.rootVisualElement.Q<VisualElement>("PlayerStatusEffectList");

        m_Player.StatusApplied += OnStatusApplied;
        m_Player.StatusRemoved += OnStatusRemoved;
        GameManager.Instance.TurnManager.OnTick += RefreshDurations;
    }

    void OnDestroy()
    {
        if (m_Player != null)
        {
            m_Player.StatusApplied -= OnStatusApplied;
            m_Player.StatusRemoved -= OnStatusRemoved;
        }
        if (GameManager.Instance != null)
            GameManager.Instance.TurnManager.OnTick -= RefreshDurations;
    }

    void OnStatusApplied(StatusEffect effect)
    {
        if (m_Slots.ContainsKey(effect.Type)) return;

        VisualElement slot = m_SlotTemplate.Instantiate().Q<VisualElement>("StatusEffectSlot");
        slot.Q<Label>("Duration").text = effect.Duration.ToString();
        m_Container.Add(slot);
        m_Slots[effect.Type] = slot;
    }

    void OnStatusRemoved(StatusEffect effect)
    {
        if (!m_Slots.TryGetValue(effect.Type, out VisualElement slot)) return;
        m_Container.Remove(slot);
        m_Slots.Remove(effect.Type);
    }

    void RefreshDurations()
    {
        foreach (StatusEffect effect in m_Player.StatusEffects)
        {
            if (m_Slots.TryGetValue(effect.Type, out VisualElement slot))
                slot.Q<Label>("Duration").text = effect.Duration.ToString();
        }
    }
}
