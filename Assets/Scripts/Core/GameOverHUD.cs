using System.Collections;
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

    [SerializeField]
    private AudioClip m_GameOverDepletedSFX;

    [SerializeField]
    private AudioClip m_GameOverDefeatedSFX;

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

    IEnumerator RevealGameOverPanelElementsCoroutine(int levels, GameOverReason reason)
    {
        yield return new WaitForSecondsRealtime(0.5f);

        string levelString = levels > 1 ? "levels" : "level";
        string reasonString =
            reason == GameOverReason.Depleted ? "YOU RAN OUT OF STAMINA" : "YOU WERE DEFEATED";

        m_GameOverLevelsTraveledLabel.text =
            $"You traveled through {levels} {levelString}, but in the end...";
        m_GameOverLevelsTraveledLabel.RemoveFromClassList("hidden");
        AudioManager.Instance.PlaySFX(m_ClickSFX);

        yield return new WaitForSecondsRealtime(0.15f);

        m_GameOverReasonLabel.text = reasonString;
        m_GameOverReasonLabel.RemoveFromClassList("hidden");
        AudioManager.Instance.PlaySFX(m_ClickSFX);

        yield return new WaitForSecondsRealtime(0.15f);

        m_RestartRunButton.RemoveFromClassList("hidden");
        AudioManager.Instance.PlaySFX(m_ClickSFX);
        yield return new WaitForSecondsRealtime(0.15f);

        m_ReturnToMenuButton.RemoveFromClassList("hidden");
        AudioManager.Instance.PlaySFX(m_ClickSFX);
        yield return new WaitForSecondsRealtime(0.15f);

        AudioClip sfx =
            reason == GameOverReason.Depleted ? m_GameOverDepletedSFX : m_GameOverDefeatedSFX;
        AudioManager.Instance.PlaySFX(sfx);
    }

    void OnGameOver(GameOverReason reason, int levels)
    {
        m_IsGameOver = true;

        m_GameOverPanel.style.display = DisplayStyle.Flex;
        m_GameOverPanel
            .schedule.Execute(() =>
                m_GameOverPanel.EnableInClassList("game-over-panel--visible", true)
            )
            .StartingIn(16);

        StartCoroutine(RevealGameOverPanelElementsCoroutine(levels, reason));
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

        m_GameOverLevelsTraveledLabel.AddToClassList("hidden");
        m_GameOverReasonLabel.AddToClassList("hidden");
        m_RestartRunButton.AddToClassList("hidden");
        m_ReturnToMenuButton.AddToClassList("hidden");
    }
}
