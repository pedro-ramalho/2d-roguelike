using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatantFloaters : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset m_FloaterTemplate;
    [SerializeField] private Vector3 m_WorldOffset = new Vector3(0, 0.5f, 0);
    [SerializeField] private float m_Duration = 0.5f;
    [SerializeField] private float m_RisePixels = 50f;

    private static readonly Color BlockLostColor = new Color(0.05f, 0.10f, 0.31f);
    private static readonly Color HealthLostColor = new Color(0.62f, 0f, 0f);
    private static readonly Color HealthAddedColor = new Color(0f, 1f, 0f);
    private static readonly Color BlockAddedColor = new Color(0.05f, 0.10f, 0.31f);
    private static readonly Color StatusAppliedColor = new Color(1f, 1f, 0f);

    private ICombatant m_Combatant;
    private VisualElement m_ParentLayer;
    private Camera m_Camera;
    private readonly List<VisualElement> m_Active = new();

    void Start()
    {
        UIDocument doc = GameManager.Instance.UIDoc;
        m_ParentLayer = doc.rootVisualElement.Q<VisualElement>("FloaterLayer");
        m_Camera = Camera.main;

        ICombatant combatant = GetComponent<ICombatant>();
        if (combatant != null) Bind(combatant);
    }

    void OnDestroy()
    {
        if (m_Combatant != null)
        {
            m_Combatant.Damaged -= OnDamaged;
            m_Combatant.HealthAdded -= OnHealthAdded;
            m_Combatant.BlockAdded -= OnBlockAdded;
            m_Combatant.StatusApplied -= OnStatusApplied;
        }
        if (m_ParentLayer != null)
        {
            foreach (VisualElement floater in m_Active)
                m_ParentLayer.Remove(floater);
        }
        m_Active.Clear();
    }

    void OnDamaged(DamageResult result)
    {
        if (result.BlockLost > 0) Spawn($"-{result.BlockLost}", BlockLostColor);
        if (result.HPLost > 0) Spawn($"-{result.HPLost}", HealthLostColor);
    }

    void OnHealthAdded(int amount) => Spawn($"+{amount}", HealthAddedColor);
    void OnBlockAdded(int amount) => Spawn($"+{amount}", BlockAddedColor);
    void OnStatusApplied(StatusEffect effect) => Spawn($"+{effect}", StatusAppliedColor);

    void Spawn(string text, Color color)
    {
        Label floater = m_FloaterTemplate.Instantiate().Q<Label>("FloaterText");
        floater.text = text;
        floater.style.color = color;

        Vector2 panelPos = RuntimePanelUtils.CameraTransformWorldToPanel(
            m_ParentLayer.panel, transform.position + m_WorldOffset, m_Camera);
        floater.style.left = panelPos.x;
        floater.style.top = panelPos.y;

        m_ParentLayer.Add(floater);
        m_Active.Add(floater);
        StartCoroutine(RiseAndFade(floater, panelPos.y));
    }

    IEnumerator RiseAndFade(Label floater, float startTop)
    {
        float elapsed = 0;
        while (elapsed < m_Duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / m_Duration;
            floater.style.top = startTop - m_RisePixels * t;
            floater.style.opacity = 1 - t;
            yield return null;
        }
        if (m_ParentLayer != null) m_ParentLayer.Remove(floater);
        m_Active.Remove(floater);
    }

    public void Bind(ICombatant combatant)
    {
        m_Combatant = combatant;
        m_Combatant.Damaged += OnDamaged;
        m_Combatant.HealthAdded += OnHealthAdded;
        m_Combatant.BlockAdded += OnBlockAdded;
        m_Combatant.StatusApplied += OnStatusApplied;
    }
}
