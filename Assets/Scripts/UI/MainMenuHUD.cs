using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuHUD : MonoBehaviour
{
    [SerializeField]
    private UIDocument m_UIDocument;

    private Button m_StartRunButton;
    private Button m_SettingsButton;
    private Button m_CreditsButton;
    private Button m_QuitButton;

    void Start()
    {
        VisualElement root = m_UIDocument.rootVisualElement;
        VisualElement buttonContainer = root.Q<VisualElement>("ButtonContainer");

        m_StartRunButton = buttonContainer.Q<Button>("StartButton");
        m_SettingsButton = buttonContainer.Q<Button>("SettingsButton");
        m_CreditsButton = buttonContainer.Q<Button>("CreditsButton");
        m_QuitButton = buttonContainer.Q<Button>("QuitButton");

        m_StartRunButton.clicked += OnStartRunButtonPress;
        m_SettingsButton.clicked += OnSettingsButtonPress;
        m_CreditsButton.clicked += OnCreditsButtonPress;
        m_QuitButton.clicked += OnQuitButtonPress;
    }

    void OnDestroy()
    {
        m_StartRunButton.clicked -= OnStartRunButtonPress;
        m_SettingsButton.clicked -= OnSettingsButtonPress;
        m_CreditsButton.clicked -= OnCreditsButtonPress;
        m_QuitButton.clicked -= OnQuitButtonPress;
    }

    void OnStartRunButtonPress()
    {
        SceneManager.LoadScene("Main");
    }

    void OnSettingsButtonPress() { /* TBD */
    }

    void OnCreditsButtonPress() { /* TBD */
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
