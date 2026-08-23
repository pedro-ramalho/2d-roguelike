using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenuHUD : MonoBehaviour
{
    [SerializeField]
    private UIDocument m_UIDocument;

    [SerializeField]
    private AudioClip m_ClickSFX;

    private VisualElement m_SettingsMenuPanel;
    private Slider m_MasterVolumeSlider;
    private Slider m_MusicVolumeSlider;
    private Slider m_SFXVolumeSlider;
    private Button m_BackButton;

    private static readonly string m_MasterVolumeMixerParam = "MasterVolume";
    private static readonly string m_MusicVolumeMixerParam = "MusicVolume";
    private static readonly string m_SFXVolumeMixerParam = "SFXVolume";

    private static readonly float m_MasterVolumeDefaultValue = 0.5f;
    private static readonly float m_MusicVolumeDefaultValue = 0.5f;
    private static readonly float m_SFXVolumeDefaultValue = 0.5f;

    public event Action Closed;

    void Start()
    {
        VisualElement root = m_UIDocument.rootVisualElement;

        m_SettingsMenuPanel = root.Q<VisualElement>("SettingsMenuPanel");
        m_MasterVolumeSlider = m_SettingsMenuPanel.Q<Slider>("MasterVolumeSlider");
        m_MusicVolumeSlider = m_SettingsMenuPanel.Q<Slider>("MusicVolumeSlider");
        m_SFXVolumeSlider = m_SettingsMenuPanel.Q<Slider>("SFXVolumeSlider");
        m_BackButton = m_SettingsMenuPanel.Q<Button>("BackButton");

        LoadSliderSettings();

        m_MasterVolumeSlider.RegisterValueChangedCallback(OnMasterVolumeChanged);
        m_MusicVolumeSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
        m_SFXVolumeSlider.RegisterValueChangedCallback(OnSFXVolumeChanged);

        m_BackButton.clicked += OnBackButtonPress;
    }

    void LoadSliderSettings()
    {
        float masterVolumeValue = PlayerPrefs.GetFloat(
            m_MasterVolumeMixerParam,
            m_MasterVolumeDefaultValue
        );
        float musicVolumeValue = PlayerPrefs.GetFloat(
            m_MusicVolumeMixerParam,
            m_MusicVolumeDefaultValue
        );
        float sfxVolumeValue = PlayerPrefs.GetFloat(m_SFXVolumeMixerParam, m_SFXVolumeDefaultValue);

        m_MasterVolumeSlider.value = masterVolumeValue;
        m_MusicVolumeSlider.value = musicVolumeValue;
        m_SFXVolumeSlider.value = sfxVolumeValue;

        AudioManager.Instance.SetVolume(m_MasterVolumeMixerParam, masterVolumeValue);
        AudioManager.Instance.SetVolume(m_MusicVolumeMixerParam, musicVolumeValue);
        AudioManager.Instance.SetVolume(m_SFXVolumeMixerParam, sfxVolumeValue);
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

    void OnMasterVolumeChanged(ChangeEvent<float> evt)
    {
        PlayerPrefs.SetFloat(m_MasterVolumeMixerParam, evt.newValue);
        AudioManager.Instance.SetVolume(m_MasterVolumeMixerParam, evt.newValue);
    }

    void OnMusicVolumeChanged(ChangeEvent<float> evt)
    {
        PlayerPrefs.SetFloat(m_MusicVolumeMixerParam, evt.newValue);
        AudioManager.Instance.SetVolume(m_MusicVolumeMixerParam, evt.newValue);
    }

    void OnSFXVolumeChanged(ChangeEvent<float> evt)
    {
        PlayerPrefs.SetFloat(m_SFXVolumeMixerParam, evt.newValue);
        AudioManager.Instance.SetVolume(m_SFXVolumeMixerParam, evt.newValue);
    }

    void OnBackButtonPress()
    {
        AudioManager.Instance.PlaySFX(m_ClickSFX);
        Show(false);
        Closed?.Invoke();
    }

    public void Show(bool show) =>
        m_SettingsMenuPanel.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
}
