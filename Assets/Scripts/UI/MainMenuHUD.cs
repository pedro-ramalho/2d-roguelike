using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

enum PanelType
{
    Buttons,
    Settings,
    Credits,
}

public class MainMenuHUD : MonoBehaviour
{
    [SerializeField]
    private UIDocument m_UIDocument;

    // Buttons Panel
    private VisualElement m_ButtonsPanel;
    private Button m_StartRunButton;
    private Button m_SettingsButton;
    private Button m_CreditsButton;
    private Button m_QuitButton;

    // Credits Panel
    private VisualElement m_CreditsPanel;
    private Button m_CreditsBackButton;

    // Settings Panel
    private VisualElement m_SettingsPanel;
    private Button m_SettingsBackButton;

    void Start()
    {
        VisualElement root = m_UIDocument.rootVisualElement;
        VisualElement mainMenuPanel = root.Q<VisualElement>("MainMenuPanel");

        m_ButtonsPanel = mainMenuPanel.Q<VisualElement>("ButtonsPanel");
        m_CreditsPanel = mainMenuPanel.Q<VisualElement>("CreditsPanel");
        m_SettingsPanel = mainMenuPanel.Q<VisualElement>("SettingsPanel");

        m_StartRunButton = m_ButtonsPanel.Q<Button>("StartRunButton");
        m_CreditsButton = m_ButtonsPanel.Q<Button>("CreditsButton");
        m_SettingsButton = m_ButtonsPanel.Q<Button>("SettingsButton");
        m_QuitButton = m_ButtonsPanel.Q<Button>("QuitButton");

        m_CreditsBackButton = m_CreditsPanel.Q<Button>("CreditsBackButton");
        m_SettingsBackButton = m_SettingsPanel.Q<Button>("SettingsBackButton");

        m_StartRunButton.clicked += OnStartRunButtonPress;
        m_CreditsButton.clicked += OnCreditsButtonPress;
        m_SettingsButton.clicked += OnSettingsButtonPress;
        m_QuitButton.clicked += OnQuitButtonPress;

        m_CreditsBackButton.clicked += OnCreditsBackButtonPress;
        m_SettingsBackButton.clicked += OnSettingsBackButtonPress;
    }

    void OnDestroy()
    {
        if (m_StartRunButton != null)
            m_StartRunButton.clicked -= OnStartRunButtonPress;

        if (m_SettingsButton != null)
            m_SettingsButton.clicked -= OnSettingsButtonPress;

        if (m_CreditsButton != null)
            m_CreditsButton.clicked -= OnCreditsButtonPress;

        if (m_QuitButton != null)
            m_QuitButton.clicked -= OnQuitButtonPress;

        if (m_CreditsBackButton != null)
            m_CreditsBackButton.clicked -= OnCreditsBackButtonPress;

        if (m_SettingsBackButton != null)
            m_SettingsBackButton.clicked -= OnSettingsBackButtonPress;
    }

    void ShowPanel(PanelType panel)
    {
        m_ButtonsPanel.style.display =
            panel == PanelType.Buttons ? DisplayStyle.Flex : DisplayStyle.None;
        m_SettingsPanel.style.display =
            panel == PanelType.Settings ? DisplayStyle.Flex : DisplayStyle.None;
        m_CreditsPanel.style.display =
            panel == PanelType.Credits ? DisplayStyle.Flex : DisplayStyle.None;
    }

    void OnStartRunButtonPress()
    {
        SceneManager.LoadScene("Main");
    }

    void OnCreditsButtonPress() => ShowPanel(PanelType.Credits);

    void OnCreditsBackButtonPress() => ShowPanel(PanelType.Buttons);

    void OnSettingsButtonPress() => ShowPanel(PanelType.Settings);

    void OnSettingsBackButtonPress() => ShowPanel(PanelType.Buttons);

    void OnQuitButtonPress()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
