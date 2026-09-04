using Combat;
using Core;
using Enemy;
using Food;
using UnityEngine;

namespace Loot
{
    public class LootDropper : MonoBehaviour
    {
        [SerializeField]
        private LootEntry[] m_LootTable;

        private Combatant m_Combatant;

        void Awake()
        {
            m_Combatant = GetComponent<Combatant>();

            m_Combatant.Defeated += OnDefeated;
        }

        void OnDestroy()
        {
            if (m_Combatant != null)
                m_Combatant.Defeated -= OnDefeated;
        }

        void OnDefeated()
        {
            LootEntry entry = WeightedPool.PickRandom(m_LootTable, e => e.Weight);
            switch (entry.Kind)
            {
                case LootKind.Nothing:
                    break;
                case LootKind.Stamina:
                    GameManager.Instance.PlayerController.Combatant.ChangeStamina(
                        entry.StaminaAmount
                    );
                    break;

                case LootKind.Food:
                    GameManager.Instance.BoardManager.Place(entry.FoodPrefab, m_Combatant.Cell);
                    break;
            }
        }
    }
}
