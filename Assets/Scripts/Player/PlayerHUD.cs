using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private UIDocument m_UIDocument;
    private Player m_Player;

    private Label m_HealthLabel;
    private Label m_BlockLabel;
    private Label m_StaminaLabel;
    private Label m_AttackLabel;

    void Awake()
    {
        m_Player = GetComponent<Player>();

        VisualElement playerStatsPanel = m_UIDocument.rootVisualElement.Q<VisualElement>("PlayerStatsPanel");        
        m_HealthLabel = playerStatsPanel.Q<Label>("HealthLabel");
        m_BlockLabel = playerStatsPanel.Q<Label>("BlockLabel");
        m_StaminaLabel = playerStatsPanel.Q<Label>("StaminaLabel");
        m_AttackLabel = playerStatsPanel.Q<Label>("AttackLabel");

        m_Player.Damaged += _ => Refresh();
        m_Player.HealthAdded += _ => Refresh();
        m_Player.BlockAdded += _ => Refresh();
        m_Player.StaminaChanged += _ => Refresh();
    }

    void Start() => Refresh();

    void OnDestroy()
    {
        if (m_Player == null) return;
        
        m_Player.Damaged += _ => Refresh();
        m_Player.HealthAdded += _ => Refresh();
        m_Player.BlockAdded += _ => Refresh();
        m_Player.StaminaChanged += _ => Refresh();
    }

    void Refresh()
    {
        m_HealthLabel.text = $"HP: {m_Player.HP}/{m_Player.MaxHP}";
        m_BlockLabel.text = $"Block: {m_Player.Block}";
        m_StaminaLabel.text = $"Stamina: {m_Player.Stamina}";
        m_AttackLabel.text = $"Attack: {m_Player.Attack}";
    }
}
