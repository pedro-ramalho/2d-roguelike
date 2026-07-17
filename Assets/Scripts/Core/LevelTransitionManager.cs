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

    IEnumerator FadeIn()
    {
        yield return null;
    }

    IEnumerator FadeOut(int levelNumber)
    {
        yield return null;
    }
}
