using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelTransitionManager : MonoBehaviour
{
    [SerializeField] private UIDocument m_UIDocument;

    // UI Elements
    private VisualElement m_LevelTransitionPanel;
    private Label m_NewLevelLabel;

    // Animation parameters
    private readonly float m_FadeDuration = 0.5f;

    void Awake()
    {
        VisualElement root = m_UIDocument.rootVisualElement;

        m_LevelTransitionPanel = root.Q<VisualElement>("LevelTransitionPanel");
        m_NewLevelLabel = m_LevelTransitionPanel.Q<Label>("NewLevelLabel");
    }

    void SetLevelText(int levelNumber) => m_NewLevelLabel.text = $"Level {levelNumber}";

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
    }

    public IEnumerator FadeOutCoroutine(int levelNumber)
    {
        SetLevelText(levelNumber);

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
