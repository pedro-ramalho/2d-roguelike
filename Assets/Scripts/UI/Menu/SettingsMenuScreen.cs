using System;
using Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class SettingsMenuScreen : MonoBehaviour
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

        public event Action Closed;

        void Start()
        {
            VisualElement root = m_UIDocument.rootVisualElement;

            m_SettingsMenuPanel = root.Q<VisualElement>("SettingsMenuPanel");
            m_MasterVolumeSlider = m_SettingsMenuPanel.Q<Slider>("MasterVolumeSlider");
            m_MusicVolumeSlider = m_SettingsMenuPanel.Q<Slider>("MusicVolumeSlider");
            m_SFXVolumeSlider = m_SettingsMenuPanel.Q<Slider>("SFXVolumeSlider");
            m_BackButton = m_SettingsMenuPanel.Q<Button>("BackButton");

            BindVolumeSlider(m_MasterVolumeSlider, "MasterVolume", 0.5f);
            BindVolumeSlider(m_MusicVolumeSlider, "MusicVolume", 0.5f);
            BindVolumeSlider(m_SFXVolumeSlider, "SFXVolume", 0.5f);

            m_BackButton.clicked += OnBackButtonPress;
        }

        void BindVolumeSlider(Slider slider, string mixerParam, float defaultValue)
        {
            float initial = PlayerPrefs.GetFloat(mixerParam, defaultValue);
            slider.value = initial;
            AudioManager.Instance.SetVolume(mixerParam, initial);

            slider.RegisterValueChangedCallback(evt =>
            {
                PlayerPrefs.SetFloat(mixerParam, evt.newValue);
                AudioManager.Instance.SetVolume(mixerParam, evt.newValue);
            });
        }

        void OnDestroy()
        {
            if (m_BackButton != null)
                m_BackButton.clicked -= OnBackButtonPress;
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
}
