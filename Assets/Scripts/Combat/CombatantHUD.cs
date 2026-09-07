using System.Collections.Generic;
using Core;
using Status;
using UnityEngine;
using UnityEngine.UIElements;

namespace Combat
{
    public class CombatantHUD : MonoBehaviour
    {
        [SerializeField]
        private VisualTreeAsset m_Template;

        [SerializeField]
        private VisualTreeAsset m_StatusSlotTemplate;

        [SerializeField]
        private bool m_ShowStatusEffects = true;

        [SerializeField]
        private Vector3 m_WorldOffset = new Vector3(0, 0.5f, 0);

        [SerializeField]
        private StatusEffectIconSet m_IconSet;

        private Combatant m_Combatant;
        private CombatantStats m_Stats;
        private CombatantStatusEffects m_Statuses;

        private VisualElement m_ParentLayer;
        private VisualElement m_Root;
        private VisualElement m_HPFill;
        private VisualElement m_BlockFill;
        private VisualElement m_StatusRow;
        private Camera m_Camera;
        private readonly Dictionary<StatusEffectType, VisualElement> m_Slots = new();

        void Awake()
        {
            m_Camera = Camera.main;

            m_Combatant = GetComponent<Combatant>();
            m_Stats = GetComponent<CombatantStats>();
            m_Statuses = GetComponent<CombatantStatusEffects>();
        }

        void CreateHUD()
        {
            UIDocument doc = GameManager.Instance.HUDLayersDoc;
            m_ParentLayer = doc.rootVisualElement.Q<VisualElement>("CombatantHUDLayer");

            VisualElement container = m_Template.Instantiate();
            m_Root = container.Q<VisualElement>("CombatantHUD");
            m_ParentLayer.Add(m_Root);

            m_HPFill = m_Root.Q<VisualElement>("HPFill");
            m_BlockFill = m_Root.Q<VisualElement>("BlockFill");
            m_StatusRow = m_Root.Q<VisualElement>("StatusEffectsRow");

            if (!m_ShowStatusEffects)
                m_StatusRow.style.display = DisplayStyle.None;
        }

        void Start()
        {
            CreateHUD();

            m_Stats.Damaged += OnStatChanged;
            m_Stats.HealthAdded += OnStatChanged;
            m_Stats.BlockAdded += OnStatChanged;
            m_Stats.StatsReset += RefreshBars;

            m_Statuses.Applied += OnStatusApplied;
            m_Statuses.Removed += OnStatusRemoved;
            m_Statuses.Ticked += OnTurnTicked;

            m_Combatant.Defeated += OnCombatantDefeated;

            RefreshBars();
        }

        void OnEnable()
        {
            if (m_Combatant != null)
                RefreshBars();
        }

        void OnDisable()
        {
            if (m_Root != null)
                m_Root.style.visibility = Visibility.Hidden;
        }

        void LateUpdate()
        {
            if (m_Root == null || m_Camera == null)
                return;

            Vector2 panelPos = RuntimePanelUtils.CameraTransformWorldToPanel(
                m_Root.panel,
                transform.position + m_WorldOffset,
                m_Camera
            );

            m_Root.style.left = panelPos.x;
            m_Root.style.top = panelPos.y;
            m_Root.style.visibility = Visibility.Visible;
        }

        void OnDestroy()
        {
            if (m_Combatant != null)
                m_Combatant.Defeated -= OnCombatantDefeated;

            if (m_Stats != null)
            {
                m_Stats.Damaged -= OnStatChanged;
                m_Stats.HealthAdded -= OnStatChanged;
                m_Stats.BlockAdded -= OnStatChanged;
                m_Stats.StatsReset -= RefreshBars;
            }

            if (m_Statuses != null)
            {
                m_Statuses.Applied -= OnStatusApplied;
                m_Statuses.Removed -= OnStatusRemoved;
                m_Statuses.Ticked -= OnTurnTicked;
            }

            if (m_Root != null && m_ParentLayer != null)
                m_ParentLayer.Remove(m_Root);
        }

        void OnStatChanged<T>(T _) => RefreshBars();

        void RefreshBars()
        {
            float hpPct = (float)m_Stats.HP / m_Stats.MaxHP * 100f;
            float blockPct = (float)m_Stats.Block / m_Stats.MaxHP * 100f;

            m_HPFill.style.width = Length.Percent(hpPct);
            m_BlockFill.style.width = Length.Percent(blockPct);
        }

        void OnStatusApplied(StatusEffect effect)
        {
            if (!m_ShowStatusEffects)
                return;

            if (m_Slots.ContainsKey(effect.Type))
                return;

            VisualElement slot = m_StatusSlotTemplate.Instantiate();

            Sprite icon = m_IconSet != null ? m_IconSet.For(effect.Type) : null;
            if (icon != null)
                slot.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(icon);

            Label duration = slot.Q<Label>("Duration");
            duration.text = effect.Duration.ToString();

            m_StatusRow.Add(slot);
            m_Slots[effect.Type] = slot;
        }

        void OnStatusRemoved(StatusEffect effect)
        {
            if (!m_Slots.TryGetValue(effect.Type, out VisualElement slot))
                return;

            m_StatusRow.Remove(slot);
            m_Slots.Remove(effect.Type);
        }

        void OnCombatantDefeated()
        {
            m_Root.style.visibility = Visibility.Hidden;
            enabled = false;
        }

        void OnTurnTicked()
        {
            foreach (StatusEffect effect in m_Statuses.StatusEffects)
                if (m_Slots.TryGetValue(effect.Type, out VisualElement slot))
                    slot.Q<Label>("Duration").text = effect.Duration.ToString();
        }
    }
}
