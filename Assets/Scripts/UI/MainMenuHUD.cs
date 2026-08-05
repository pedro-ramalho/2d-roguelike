using UnityEngine;
using UnityEngine.UIElements;

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
    }

    void OnStartRunButtonPress() { }

    void OnSettingsButtonPress() { }

    void OnCreditsButtonPress() { }

    void OnQuitButtonPress()
    {
        Application.Quit();
    }
}
