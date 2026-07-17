using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelTransitionManager : MonoBehaviour
{
    [SerializeField] private UIDocument m_UIDocument;

    // UI Elements
    private VisualElement m_LevelTransitionPanel;

    // Animation parameters
    private readonly float m_FadeDuration = 0.5f;

    void Awake()
    {
        VisualElement root = m_UIDocument.rootVisualElement;

        m_LevelTransitionPanel = root.Q<VisualElement>("LevelTransitionPanel");
    }

    public IEnumerator FadeIn()
    {
        float elapsed = 0;

        while (elapsed < m_FadeDuration)
        {
            elapsed += Time.deltaTime;

            float opacity = m_LevelTransitionPanel.style.opacity.value;

            m_LevelTransitionPanel.style.opacity = Mathf.Lerp(opacity, 1, elapsed / m_FadeDuration);

            yield return null;    
        }
    }

    public IEnumerator FadeOut(int levelNumber)
    {
        float elapsed = 0;

        while (elapsed < m_FadeDuration)
        {
            elapsed += Time.deltaTime;

            float opacity = m_LevelTransitionPanel.style.opacity.value;

            m_LevelTransitionPanel.style.opacity = Mathf.Lerp(1, opacity, elapsed / m_FadeDuration);

            yield return null;    
        }
    }
}
