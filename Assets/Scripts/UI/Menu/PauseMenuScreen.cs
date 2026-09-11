using Core;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI
{
    public class PauseMenuScreen : MonoBehaviour
    {
        [SerializeField]
        private UIDocument m_UIDocument;

        [SerializeField]
        [FormerlySerializedAs("m_SettingsMenuHUD")]
        private SettingsMenuScreen m_SettingsMenuScreen;

        [SerializeField]
        private AudioClip m_ClickSFX;

        private VisualElement m_PauseMenuPanel;
        private Button m_ResumeButton;
        private Button m_SettingsButton;
        private Button m_QuitButton;

        private PlayerInputActions m_InputActions;

        private bool m_IsPaused;

        void Start()
        {
            m_InputActions = new PlayerInputActions();

            VisualElement root = m_UIDocument.rootVisualElement;

            m_PauseMenuPanel = root.Q<VisualElement>("PauseMenuPanel");
            m_ResumeButton = m_PauseMenuPanel.Q<Button>("ResumeButton");
            m_SettingsButton = m_PauseMenuPanel.Q<Button>("SettingsButton");
            m_QuitButton = m_PauseMenuPanel.Q<Button>("QuitButton");

            m_ResumeButton.clicked += OnResumeButtonPress;
            m_SettingsButton.clicked += OnSettingsButtonPress;
            m_QuitButton.clicked += OnQuitButtonPress;

            m_SettingsMenuScreen.Closed += OnSettingsMenuClosed;
        }

        void OnEnable() => m_InputActions.Enable();

        void OnDisable() => m_InputActions.Disable();

        void OnDestroy()
        {
            if (m_ResumeButton != null)
                m_ResumeButton.clicked -= OnResumeButtonPress;

            if (m_SettingsButton != null)
                m_SettingsButton.clicked -= OnSettingsButtonPress;

            if (m_QuitButton != null)
                m_QuitButton.clicked -= OnQuitButtonPress;

            if (m_SettingsMenuScreen != null)
                m_SettingsMenuScreen.Closed -= OnSettingsMenuClosed;
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

        void PlayClickSFX() => AudioManager.Instance.PlaySFX(m_ClickSFX);

        void ShowPauseButtons(bool show)
        {
            m_ResumeButton.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            m_SettingsButton.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            m_QuitButton.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }

        void OnResumeButtonPress()
        {
            PlayClickSFX();
            Pause(false);
        }

        void OnSettingsButtonPress()
        {
            PlayClickSFX();

            ShowPauseButtons(false);

            m_SettingsMenuScreen.Show(true);
        }

        void OnSettingsMenuClosed() => ShowPauseButtons(true);

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
