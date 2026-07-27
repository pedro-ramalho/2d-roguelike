using UnityEngine;
using UnityEngine.UIElements;

public class GameOverHUD : MonoBehaviour
{
    private VisualElement m_GameOverPanel;
    private Label m_GameOverMessage;

    // UI document
    [SerializeField] private UIDocument m_UIDocument;
    [SerializeField] private LevelManager m_LevelManager;

    void Start()
    {
        VisualElement root = m_UIDocument.rootVisualElement;

        m_GameOverPanel = root.Q<VisualElement>("GameOverPanel");
        m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");

        m_GameOverPanel.style.visibility = Visibility.Hidden;
        
        m_LevelManager.GameOverTriggered += OnGameOver;
        m_LevelManager.GameStartTriggered += OnGameStart;
    }

    void OnDestroy()
    {
        if (m_LevelManager != null)
        {
            m_LevelManager.GameOverTriggered -= OnGameOver;
            m_LevelManager.GameStartTriggered -= OnGameStart;
        }
    }

    void OnGameOver(GameOverReason reason, int levels)
    {
        string levelString = levels > 1 ? "levels" : "level";
        string reasonString = reason == GameOverReason.Depleted ? "You ran out of stamina!" : "You were defeated!";

        m_GameOverPanel.style.visibility = Visibility.Visible;
        m_GameOverMessage.text = $"Game Over! {reasonString}\n\nYou traveled through {levels} {levelString}.\n\nPress Enter to restart.";
    }

    public void OnGameStart() => m_GameOverPanel.style.visibility = Visibility.Hidden;
}
