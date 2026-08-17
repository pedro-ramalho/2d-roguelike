using UnityEngine;

public class EliteModifier : MonoBehaviour
{
    private Combatant m_Combatant;

    void Awake()
    {
        m_Combatant = GetComponent<Combatant>();
        m_Combatant.ApplyStatMultiplier(1.5f, 1.25f, 1.2f);

        // Update the tint
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(138f / 255f, 43f / 255f, 226f / 255f);

        m_Combatant.Defeated += OnEliteDefeated;
    }

    void OnDestroy()
    {
        if (m_Combatant != null)
            m_Combatant.Defeated -= OnEliteDefeated;
    }

    void OnEliteDefeated()
    {
        var pool = GameManager.Instance.ProgressionSettings.EliteRewardPool;
        if (pool.Length == 0)
            return;

        EliteRewardEntry entry = WeightedPool.PickRandom(pool, e => e.Weight);
        m_Combatant.UpgradeStat(entry.Stat, entry.Amount);
    }
}
