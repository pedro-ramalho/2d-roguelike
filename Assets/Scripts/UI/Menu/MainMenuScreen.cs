using Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI
{
    enum PanelType
    {
        Buttons,
        Settings,
        Credits,
    }

    public class MainMenuScreen : MonoBehaviour
    {
        [SerializeField]
        private UIDocument m_UIDocument;

        [SerializeField]
        private SettingsMenuScreen m_SettingsMenuHUD;

        [SerializeField]
        private AudioClip m_ClickSFX;

        // Game Title Label
        private Label m_GameTitleLabel;

        // Buttons Panel
        private VisualElement m_ButtonsPanel;
        private Button m_StartRunButton;
        private Button m_SettingsButton;
        private Button m_CreditsButton;
        private Button m_QuitButton;

        // Credits Panel
        private VisualElement m_CreditsPanel;
        private Button m_CreditsBackButton;

        void Start()
        {
            VisualElement root = m_UIDocument.rootVisualElement;
            VisualElement mainMenuPanel = root.Q<VisualElement>("MainMenuPanel");

            m_GameTitleLabel = mainMenuPanel.Q<Label>("GameTitleLabel");

            m_ButtonsPanel = mainMenuPanel.Q<VisualElement>("ButtonsPanel");
            m_CreditsPanel = mainMenuPanel.Q<VisualElement>("CreditsPanel");

            m_StartRunButton = m_ButtonsPanel.Q<Button>("StartRunButton");
            m_CreditsButton = m_ButtonsPanel.Q<Button>("CreditsButton");
            m_SettingsButton = m_ButtonsPanel.Q<Button>("SettingsButton");
            m_QuitButton = m_ButtonsPanel.Q<Button>("QuitButton");

            m_CreditsBackButton = m_CreditsPanel.Q<Button>("CreditsBackButton");

            m_StartRunButton.clicked += OnStartRunButtonPress;
            m_CreditsButton.clicked += OnCreditsButtonPress;
            m_SettingsButton.clicked += OnSettingsButtonPress;
            m_QuitButton.clicked += OnQuitButtonPress;

            m_CreditsBackButton.clicked += OnCreditsBackButtonPress;

            m_SettingsMenuHUD.Closed += OnSettingsMenuClosed;
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

            if (m_SettingsMenuHUD != null)
                m_SettingsMenuHUD.Closed -= OnSettingsMenuClosed;
        }

        void ShowPanel(PanelType panel)
        {
            m_GameTitleLabel.style.display =
                panel == PanelType.Buttons ? DisplayStyle.Flex : DisplayStyle.None;

            m_ButtonsPanel.style.display =
                panel == PanelType.Buttons ? DisplayStyle.Flex : DisplayStyle.None;

            m_SettingsMenuHUD.Show(panel == PanelType.Settings);

            m_CreditsPanel.style.display =
                panel == PanelType.Credits ? DisplayStyle.Flex : DisplayStyle.None;
        }

        void PlayClickSFX() => AudioManager.Instance.PlaySFX(m_ClickSFX);

        void OnStartRunButtonPress()
        {
            PlayClickSFX();
            SceneManager.LoadScene("Main");
        }

        void OnCreditsButtonPress()
        {
            PlayClickSFX();
            ShowPanel(PanelType.Credits);
        }

        void OnCreditsBackButtonPress()
        {
            PlayClickSFX();
            ShowPanel(PanelType.Buttons);
        }

        void OnSettingsButtonPress()
        {
            PlayClickSFX();
            ShowPanel(PanelType.Settings);
        }

        void OnSettingsMenuClosed() => ShowPanel(PanelType.Buttons);

        void OnQuitButtonPress()
        {
            PlayClickSFX();
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}
