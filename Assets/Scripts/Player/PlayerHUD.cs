using Combat;
using Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Player
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField]
        private UIDocument m_UIDocument;
        private CombatantStats m_Stats;

        private VisualElement m_PlayerStatsPanel;
        private Label m_LevelLabel;
        private Label m_HealthLabel;
        private Label m_BlockLabel;
        private Label m_StaminaLabel;
        private Label m_AttackLabel;

        void Awake()
        {
            m_Stats = GetComponent<CombatantStats>();

            m_PlayerStatsPanel = m_UIDocument.rootVisualElement.Q<VisualElement>(
                "PlayerStatsPanel"
            );
            m_LevelLabel = m_PlayerStatsPanel.Q<Label>("LevelLabel");
            m_HealthLabel = m_PlayerStatsPanel.Q<Label>("HealthLabel");
            m_BlockLabel = m_PlayerStatsPanel.Q<Label>("BlockLabel");
            m_StaminaLabel = m_PlayerStatsPanel.Q<Label>("StaminaLabel");
            m_AttackLabel = m_PlayerStatsPanel.Q<Label>("AttackLabel");

            m_Stats.Damaged += _ => Refresh();
            m_Stats.HealthAdded += _ => Refresh();
            m_Stats.BlockAdded += _ => Refresh();
            m_Stats.StaminaChanged += _ => Refresh();
            m_Stats.StatsReset += Refresh;
        }

        void Start()
        {
            GameManager.Instance.LevelManager.LevelChanged += OnLevelChanged;
            Refresh();
        }

        void OnLevelChanged(int level) => m_LevelLabel.text = $"LV {level}";

        void OnDisable()
        {
            if (m_PlayerStatsPanel != null)
                m_PlayerStatsPanel.style.display = DisplayStyle.None;
        }

        void OnEnable()
        {
            if (m_PlayerStatsPanel != null)
                m_PlayerStatsPanel.style.display = DisplayStyle.Flex;
        }

        void OnDestroy()
        {
            if (GameManager.Instance != null && GameManager.Instance.LevelManager != null)
                GameManager.Instance.LevelManager.LevelChanged -= OnLevelChanged;

            if (m_Stats == null)
                return;

            m_Stats.Damaged -= _ => Refresh();
            m_Stats.HealthAdded -= _ => Refresh();
            m_Stats.BlockAdded -= _ => Refresh();
            m_Stats.StaminaChanged -= _ => Refresh();
            m_Stats.StatsReset -= Refresh;
        }

        void Refresh()
        {
            m_HealthLabel.text = $"{m_Stats.HP}/{m_Stats.MaxHP}";
            m_BlockLabel.text = $"{m_Stats.Block}";
            m_StaminaLabel.text = $"{m_Stats.Stamina}";
            m_AttackLabel.text = $"{m_Stats.Attack}";
        }
    }
}
