using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameOverHUD : MonoBehaviour
{
    private PlayerInputActions m_InputActions;

    private VisualElement m_GameOverPanel;
    private Label m_GameOverMessage;
    private Button m_RestartRunButton;
    private Button m_ReturnToMenuButton;

    private bool m_IsGameOver;

    // UI document
    [SerializeField]
    private UIDocument m_UIDocument;

    [SerializeField]
    private LevelManager m_LevelManager;

    [SerializeField]
    private AudioClip m_ClickSFX;

    void Awake() => m_InputActions = new PlayerInputActions();

    void Start()
    {
        m_IsGameOver = false;

        VisualElement root = m_UIDocument.rootVisualElement;

        m_GameOverPanel = root.Q<VisualElement>("GameOverPanel");
        m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");
        m_RestartRunButton = m_GameOverPanel.Q<Button>("RestartRunButton");
        m_ReturnToMenuButton = m_GameOverPanel.Q<Button>("ReturnToMenuButton");

        m_GameOverPanel.style.visibility = Visibility.Hidden;

        m_LevelManager.GameOverTriggered += OnGameOver;
        m_LevelManager.GameStartTriggered += OnGameStart;

        m_RestartRunButton.clicked += OnRestartRunButtonPress;
        m_ReturnToMenuButton.clicked += OnReturnToMenuButtonPress;
    }

    void Update()
    {
        if (m_IsGameOver && m_InputActions.Player.Restart.WasPressedThisFrame())
            GameManager.Instance.LevelManager.StartNewGame();
    }

    void OnEnable() => m_InputActions.Player.Enable();

    void OnDisable() => m_InputActions.Player.Disable();

    void OnDestroy()
    {
        m_InputActions.Dispose();

        if (m_LevelManager != null)
        {
            m_LevelManager.GameOverTriggered -= OnGameOver;
            m_LevelManager.GameStartTriggered -= OnGameStart;
        }

        if (m_RestartRunButton != null)
            m_RestartRunButton.clicked -= OnRestartRunButtonPress;

        if (m_ReturnToMenuButton != null)
            m_ReturnToMenuButton.clicked -= OnReturnToMenuButtonPress;
    }

    void OnGameOver(GameOverReason reason, int levels)
    {
        m_IsGameOver = true;

        string levelString = levels > 1 ? "levels" : "level";
        string reasonString =
            reason == GameOverReason.Depleted ? "You ran out of stamina!" : "You were defeated!";

        m_GameOverPanel.style.visibility = Visibility.Visible;
        m_GameOverMessage.text =
            $"Game Over! {reasonString}\n\nYou traveled through {levels} {levelString}.\n\nPress Enter to restart.";
    }

    void OnRestartRunButtonPress()
    {
        AudioManager.Instance.PlaySFX(m_ClickSFX);
        GameManager.Instance.LevelManager.StartNewGame();
    }

    void OnReturnToMenuButtonPress()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.PlaySFX(m_ClickSFX);
        SceneManager.LoadScene("Menu");
    }

    void OnGameStart()
    {
        m_IsGameOver = false;
        m_GameOverPanel.style.visibility = Visibility.Hidden;
    }
}
