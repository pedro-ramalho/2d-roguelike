using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenuHUD : MonoBehaviour
{
    [SerializeField]
    private UIDocument m_UIDocument;

    private VisualElement m_SettingsMenuPanel;
    private Slider m_MasterVolumeSlider;
    private Slider m_MusicVolumeSlider;
    private Slider m_SFXVolumeSlider;
    private Button m_BackButton;

    void Start()
    {
        VisualElement root = m_UIDocument.rootVisualElement;

        m_SettingsMenuPanel = root.Q<VisualElement>("SettingsMenuPanel");
        m_MasterVolumeSlider = m_SettingsMenuPanel.Q<Slider>("MasterVolumeSlider");
        m_MusicVolumeSlider = m_SettingsMenuPanel.Q<Slider>("MusicVolumeSlider");
        m_SFXVolumeSlider = m_SettingsMenuPanel.Q<Slider>("SFXVolumeSlider");
        m_BackButton = m_SettingsMenuPanel.Q<Button>("BackButton");

        m_MasterVolumeSlider.RegisterValueChangedCallback(OnMasterVolumeChanged);
        m_MusicVolumeSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
        m_SFXVolumeSlider.RegisterValueChangedCallback(OnSFXVolumeChanged);

        m_BackButton.clicked += OnBackButtonPress;
    }

    void OnDestroy()
    {
        if (m_MasterVolumeSlider != null)
            m_MasterVolumeSlider.UnregisterValueChangedCallback(OnMasterVolumeChanged);

        if (m_MusicVolumeSlider != null)
            m_MusicVolumeSlider.UnregisterValueChangedCallback(OnMusicVolumeChanged);

        if (m_SFXVolumeSlider != null)
            m_SFXVolumeSlider.UnregisterValueChangedCallback(OnSFXVolumeChanged);

        if (m_BackButton != null)
            m_BackButton.clicked -= OnBackButtonPress;
    }

    void OnMasterVolumeChanged(ChangeEvent<float> evt) =>
        AudioManager.Instance.SetVolume("MasterVolume", evt.newValue);

    void OnMusicVolumeChanged(ChangeEvent<float> evt) =>
        AudioManager.Instance.SetVolume("MusicVolume", evt.newValue);

    void OnSFXVolumeChanged(ChangeEvent<float> evt) =>
        AudioManager.Instance.SetVolume("SFXVolume", evt.newValue);

    void OnBackButtonPress() => Show(false);

    public void Show(bool show) =>
        m_SettingsMenuPanel.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
}
