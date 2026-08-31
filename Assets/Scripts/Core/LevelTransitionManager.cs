using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelTransitionManager : MonoBehaviour
{
    [SerializeField]
    private UIDocument m_UIDocument;

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
        m_NewLevelLabel.text = $"Levels {band.MinLevel} - {band.MaxLevel}";
        m_BandNameLabel.text = band.Name;
    }

    public IEnumerator FadeInCoroutine()
    {
        float elapsed = 0;
        float opacity = m_LevelTransitionPanel.style.opacity.value;

        while (elapsed < m_FadeDuration)
        {
            elapsed += Time.deltaTime;

            m_LevelTransitionPanel.style.opacity = Mathf.Lerp(opacity, 0, elapsed / m_FadeDuration);

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

        while (elapsed < m_FadeDuration)
        {
            elapsed += Time.deltaTime;

            m_LevelTransitionPanel.style.opacity = Mathf.Lerp(opacity, 1, elapsed / m_FadeDuration);

            yield return null;
        }
    }
}
