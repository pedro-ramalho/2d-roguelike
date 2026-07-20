using UnityEngine;
using UnityEngine.UIElements;

public class GameOverHUD : MonoBehaviour
{
    private VisualElement m_GameOverPanel;
    private Label m_GameOverMessage;
    private GameOverReason m_GameOverReason;

    // Private references
    private PlayerController m_PlayerController;

    // UI document
    [SerializeField] private UIDocument m_UIDocument;

    void Start()
    {
        VisualElement root = m_UIDocument.rootVisualElement;

        m_GameOverPanel = root.Q<VisualElement>("GameOverPanel");
        m_GameOverMessage = root.Q<Label>("GameOverMessage");

        m_GameOverPanel.style.visibility = Visibility.Hidden;

        m_PlayerController = GameManager.Instance.PlayerController;        
        m_PlayerController.PlayerStats.Depleted += OnPlayerDepleted;
        m_PlayerController.Combatant.Defeated += OnPlayerDefeated;
    }

    void OnPlayerDepleted() => TriggerGameOver(GameOverReason.Depleted, 1);
    void OnPlayerDefeated() => TriggerGameOver(GameOverReason.Defeated, 1);

    void TriggerGameOver(GameOverReason reason, int levels)
    {
        m_PlayerController.GameOver();

        string levelString = levels > 1 ? "levels" : "level";
        string reasonString = reason == GameOverReason.Depleted ? "You ran out of stamina!" : "You were defeated!";

        m_GameOverPanel.style.visibility = Visibility.Visible;
        m_GameOverMessage.text = $"Game Over! {reasonString}\n\nYou traveled through {levels} {levelString}.\n\nPress Enter to restart.";
        m_GameOverReason = reason;
    }
}
