using UnityEngine;

public class CombatantFloatersHUD : MonoBehaviour
{
    private ICombatant m_Combatant;

    void OnDamaged(DamageResult result)
    {
        
    }

    void OnHealthAdded(int amount)
    {
        
    }

    void OnBlockAdded(int amount)
    {
        
    }

    public void Bind(ICombatant combatant)
    {
        m_Combatant = combatant;

        m_Combatant.Damaged += OnDamaged;
        m_Combatant.HealthAdded += OnHealthAdded;
        m_Combatant.BlockAdded += OnBlockAdded;
    }
}
