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
        private Combatant m_Player;

        private VisualElement m_PlayerStatsPanel;
        private Label m_LevelLabel;
        private Label m_HealthLabel;
        private Label m_BlockLabel;
        private Label m_StaminaLabel;
        private Label m_AttackLabel;

        void Awake()
        {
            m_Player = GetComponent<Combatant>();

            m_PlayerStatsPanel = m_UIDocument.rootVisualElement.Q<VisualElement>(
                "PlayerStatsPanel"
            );
            m_LevelLabel = m_PlayerStatsPanel.Q<Label>("LevelLabel");
            m_HealthLabel = m_PlayerStatsPanel.Q<Label>("HealthLabel");
            m_BlockLabel = m_PlayerStatsPanel.Q<Label>("BlockLabel");
            m_StaminaLabel = m_PlayerStatsPanel.Q<Label>("StaminaLabel");
            m_AttackLabel = m_PlayerStatsPanel.Q<Label>("AttackLabel");

            m_Player.Damaged += _ => Refresh();
            m_Player.HealthAdded += _ => Refresh();
            m_Player.BlockAdded += _ => Refresh();
            m_Player.StaminaChanged += _ => Refresh();
            m_Player.StateReset += Refresh;
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

            if (m_Player == null)
                return;

            m_Player.Damaged -= _ => Refresh();
            m_Player.HealthAdded -= _ => Refresh();
            m_Player.BlockAdded -= _ => Refresh();
            m_Player.StaminaChanged -= _ => Refresh();
            m_Player.StateReset -= Refresh;
        }

        void Refresh()
        {
            m_HealthLabel.text = $"{m_Player.HP}/{m_Player.MaxHP}";
            m_BlockLabel.text = $"{m_Player.Block}";
            m_StaminaLabel.text = $"{m_Player.Stamina}";
            m_AttackLabel.text = $"{m_Player.Attack}";
        }
    }
}
