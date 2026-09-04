using System.Collections;
using Level;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core
{
    public class LevelTransitionManager : MonoBehaviour
    {
        [SerializeField]
        private UIDocument m_UIDocument;

        [Header("Typewriter Settings")]
        [SerializeField]
        private float m_TypeInterval = 0.04f;

        [SerializeField]
        private AudioClip m_TypeSFX;

        [SerializeField]
        private AudioClip m_CassetteSFX;

        // UI Elements
        private VisualElement m_LevelTransitionPanel;
        private Label m_NewLevelLabel;
        private Label m_BandNameLabel;

        // Animation parameters
        private readonly float m_FadeDuration = 0.5f;

        void Awake()
        {
            VisualElement root = m_UIDocument.rootVisualElement;

            m_LevelTransitionPanel = root.Q<VisualElement>("LevelTransitionPanel");

            m_NewLevelLabel = m_LevelTransitionPanel.Q<Label>("NewLevelLabel");
            m_BandNameLabel = m_LevelTransitionPanel.Q<Label>("BandNameLabel");
        }

        void SetLevelTransitionText(LevelBand band)
        {
            m_NewLevelLabel.text = "";
            StartCoroutine(TypewriterCoroutine($"Levels {band.MinLevel} - {band.MaxLevel}"));

            m_BandNameLabel.text = band.Name;
        }

        IEnumerator TypewriterCoroutine(string text)
        {
            yield return new WaitForSeconds(0.5f);

            for (int i = 1; i <= text.Length; i++)
            {
                m_NewLevelLabel.text = text.Substring(0, i);
                char c = text[i - 1];

                if (!char.IsWhiteSpace(c))
                {
                    if (AudioManager.Instance != null && m_TypeSFX != null)
                        AudioManager.Instance.PlaySFXWithPitch(
                            m_TypeSFX,
                            1f + Random.Range(-0.08f, 0.08f)
                        );
                }

                yield return new WaitForSeconds(m_TypeInterval);
            }
        }

        public IEnumerator FadeInCoroutine()
        {
            float elapsed = 0;
            float opacity = m_LevelTransitionPanel.style.opacity.value;

            while (elapsed < m_FadeDuration)
            {
                elapsed += Time.deltaTime;

                m_LevelTransitionPanel.style.opacity = Mathf.Lerp(
                    opacity,
                    0,
                    elapsed / m_FadeDuration
                );

                yield return null;
            }

            m_LevelTransitionPanel.style.opacity = 0f;
        }

        public IEnumerator FadeOutCoroutine(LevelBand band)
        {
            SetLevelTransitionText(band);

            if (band.Type == BandType.Tutorial)
            {
                m_LevelTransitionPanel.style.opacity = 1f;

                yield break;
            }

            float elapsed = 0;
            float opacity = m_LevelTransitionPanel.style.opacity.value;

            AudioManager.Instance.PlaySFX(m_CassetteSFX);

            yield return new WaitForSeconds(0.5f);

            while (elapsed < m_FadeDuration)
            {
                elapsed += Time.deltaTime;

                m_LevelTransitionPanel.style.opacity = Mathf.Lerp(
                    opacity,
                    1,
                    elapsed / m_FadeDuration
                );

                yield return null;
            }
        }
    }
}
