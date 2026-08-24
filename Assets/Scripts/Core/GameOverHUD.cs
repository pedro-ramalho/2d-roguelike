using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameOverHUD : MonoBehaviour
{
    private PlayerInputActions m_InputActions;

    private VisualElement m_GameOverPanel;
    private Label m_GameOverLevelsTraveledLabel;
    private Label m_GameOverReasonLabel;
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

        VisualElement labelsContainer = m_GameOverPanel.Q<VisualElement>(
            "GameOverPanelLabelsContainer"
        );
        VisualElement buttonsContainer = m_GameOverPanel.Q<VisualElement>(
            "GameOverPanelButtonsContainer"
        );

        m_GameOverLevelsTraveledLabel = labelsContainer.Q<Label>("GameOverLevelsTraveledLabel");
        m_GameOverReasonLabel = labelsContainer.Q<Label>("GameOverReasonLabel");
        m_RestartRunButton = buttonsContainer.Q<Button>("RestartRunButton");
        m_ReturnToMenuButton = buttonsContainer.Q<Button>("ReturnToMenuButton");

        m_GameOverPanel.style.display = DisplayStyle.None;

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
            reason == GameOverReason.Depleted ? "YOU RAN OUT OF STAMINA" : "YOU WERE DEFEATED";

        m_GameOverPanel.style.display = DisplayStyle.Flex;
        m_GameOverPanel
            .schedule.Execute(() =>
                m_GameOverPanel.EnableInClassList("game-over-panel--visible", true)
            )
            .StartingIn(16);

        m_GameOverLevelsTraveledLabel.text =
            $"You traveled through {levels} {levelString}, but in the end...";
        m_GameOverReasonLabel.text =
            reason == GameOverReason.Depleted ? "YOU RAN OUT OF STAMINA" : "YOU WERE DEFEATED";
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
        m_GameOverPanel.EnableInClassList("game-over-panel--visible", false);
        m_GameOverPanel.style.display = DisplayStyle.None;
    }
}
