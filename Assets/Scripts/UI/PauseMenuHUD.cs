using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenuHUD : MonoBehaviour
{
    [SerializeField]
    private UIDocument m_UIDocument;

    private VisualElement m_PauseMenuPanel;
    private Button m_ResumeButton;
    private Button m_SettingsButton;
    private Button m_QuitButton;

    private PlayerInputActions m_InputActions;

    private bool m_IsPaused;

    void Awake()
    {
        m_InputActions = new PlayerInputActions();

        m_IsPaused = false;

        VisualElement root = m_UIDocument.rootVisualElement;

        m_PauseMenuPanel = root.Q<VisualElement>("PauseMenuPanel");
        m_ResumeButton = m_PauseMenuPanel.Q<Button>("ResumeButton");
        m_SettingsButton = m_PauseMenuPanel.Q<Button>("SettingsButton");
        m_QuitButton = m_PauseMenuPanel.Q<Button>("QuitButton");

        m_ResumeButton.clicked += OnResumeButtonPress;
        m_SettingsButton.clicked += OnSettingsButtonPress;
        m_QuitButton.clicked += OnQuitButtonPress;
    }

    void OnDestroy()
    {
        if (m_ResumeButton != null)
            m_ResumeButton.clicked -= OnResumeButtonPress;

        if (m_SettingsButton != null)
            m_SettingsButton.clicked -= OnSettingsButtonPress;

        if (m_QuitButton != null)
            m_QuitButton.clicked -= OnQuitButtonPress;
    }

    void Pause(bool pause)
    {
        m_IsPaused = pause;

        m_PauseMenuPanel.style.display = m_IsPaused ? DisplayStyle.Flex : DisplayStyle.None;
        Time.timeScale = m_IsPaused ? 0f : 1f;
    }

    void Update()
    {
        if (m_InputActions.Player.Pause.WasPressedThisFrame())
            Pause(!m_IsPaused);
    }

    void OnResumeButtonPress() => Pause(false);

    void OnSettingsButtonPress()
    {
        // TODO: For now, this does nothing since there is no settings panel.
    }

    void OnQuitButtonPress()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
