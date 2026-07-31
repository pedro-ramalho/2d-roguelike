using System;
using UnityEngine;

public class EliteModifier : MonoBehaviour
{
    private Combatant m_Combatant;

    void Awake()
    {
        m_Combatant = GetComponent<Combatant>();
        m_Combatant.ApplyStatMultiplier(2f);

        // Update the tint
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(138f/255f, 43f/255f, 226f/255f);

        m_Combatant.Defeated += OnEliteDefeated;
    }

    void OnDestroy()
    { 
        if (m_Combatant != null)
            m_Combatant.Defeated -= OnEliteDefeated;
    }

    void OnEliteDefeated()
    {
        CombatantStat[] stats = (CombatantStat[])Enum.GetValues(typeof(CombatantStat));
    
        CombatantStat chosen = stats[UnityEngine.Random.Range(0, stats.Length)];
        GameManager.Instance.PlayerController.Combatant.UpgradeStat(chosen, 5);
    }
}
