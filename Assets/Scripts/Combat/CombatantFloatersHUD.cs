using UnityEngine;

public class CombatantFloatersHUD : MonoBehaviour
{
    [SerializeField] CombatantFloater m_FloaterPrefab;
    private ICombatant m_Combatant;

    void OnDamaged(DamageResult result)
    {
        CombatantFloater f;

        int blockLost = result.BlockLost;
        int healthLost = result.HPLost;

        if (blockLost > 0)
        {
            f = Instantiate(m_FloaterPrefab, transform);
            f.Show($"-{healthLost}", new Color(0.05139729f, 0.09887581f, 0.3113208f));
        }

        if (healthLost > 0)
        {
            f = Instantiate(m_FloaterPrefab, transform);
            f.Show($"-{blockLost}", new Color(0.622f, 0, 0));
        }
    }

    void OnHealthAdded(int amount)
    {
        CombatantFloater f;

        f = Instantiate(m_FloaterPrefab, transform);
        f.Show($"+{amount}", new Color(34, 139, 34));
    }

    void OnBlockAdded(int amount)
    {
        CombatantFloater f;

        f = Instantiate(m_FloaterPrefab, transform);
        f.Show($"+{amount}", new Color(0.05139729f, 0.09887581f, 0.3113208f));
    }

    void OnStatusApplied(StatusEffect effect)
    {
        CombatantFloater f;

        f = Instantiate(m_FloaterPrefab, transform);
        f.Show($"+{effect}", new Color(204, 204, 0)); 
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
