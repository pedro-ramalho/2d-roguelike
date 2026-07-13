using UnityEngine;
using UnityEngine.UI;

public class CombatantBarsHUD : MonoBehaviour
{
    [SerializeField] private Image m_HPBar;
    [SerializeField] private Image m_BlockBar;
    
    private ICombatant m_Combatant;

    void OnInteraction<T>(T _) => Refresh();
        
    void Refresh()
    {
        m_BlockBar.fillAmount = (float)m_Combatant.Block / m_Combatant.MaxHP; 
        m_HPBar.fillAmount = (float)m_Combatant.HP / m_Combatant.MaxHP;
    }

    void OnDestroy()
    {
        if (m_Combatant == null)
            return;

        m_Combatant.Damaged -= OnInteraction;
        m_Combatant.HealthAdded -= OnInteraction;
        m_Combatant.BlockAdded -= OnInteraction;
    }

    public void Bind(ICombatant combatant)
    {
        m_Combatant = combatant;

        m_Combatant.Damaged += OnInteraction;
        m_Combatant.HealthAdded += OnInteraction;
        m_Combatant.BlockAdded += OnInteraction;

        Refresh();
    }
}
