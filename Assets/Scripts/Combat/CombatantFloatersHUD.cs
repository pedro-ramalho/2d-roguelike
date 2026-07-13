using UnityEngine;

public class CombatantFloatersHUD : MonoBehaviour
{
    [SerializeField] CombatantFloater m_FloaterPrefab;
    private ICombatant m_Combatant;

    private readonly Color m_BlockLostColor = new Color(0.05139729f, 0.09887581f, 0.3113208f);
    private readonly Color m_HealthLostColor = new Color(0.622f, 0, 0);
    private readonly Color m_HealthAddedColor = new Color(0f, 1f, 0f);
    private readonly Color m_BlockAddedColor = new Color(0.05139729f, 0.09887581f, 0.3113208f);
    private readonly Color m_StatusAppliedColor = new Color(204, 204, 0);

    void OnDamaged(DamageResult result)
    {
        if (result.BlockLost > 0) InstantiateAndShow($"-{result.BlockLost}", m_BlockLostColor);
        if (result.HPLost > 0) InstantiateAndShow($"-{result.HPLost}", m_HealthLostColor);
    }

    void OnHealthAdded(int amount) => InstantiateAndShow($"+{amount}", m_HealthAddedColor);

    void OnBlockAdded(int amount) => InstantiateAndShow($"+{amount}", m_BlockAddedColor);

    void OnStatusApplied(StatusEffect effect) => InstantiateAndShow($"+{effect}", m_StatusAppliedColor);

    void InstantiateAndShow(string text, Color color)
    {
        CombatantFloater floater = Instantiate(m_FloaterPrefab, transform);
        floater.Show(text, color);
    }

    void OnDestroy()
    {
        if (m_Combatant == null) return;

        m_Combatant.Damaged -= OnDamaged;
        m_Combatant.HealthAdded -= OnHealthAdded;
        m_Combatant.BlockAdded -= OnBlockAdded;
        m_Combatant.StatusApplied -= OnStatusApplied; 
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
