using Board;
using Core;
using Level;
using Player;
using UnityEngine;

namespace Food
{
    public abstract class FoodObject : CellObject
    {
        [SerializeField]
        private BandAmount[] m_BandAmounts;

        protected abstract void ApplyEffect(PlayerController player);

        protected int GetAmountForCurrentBand()
        {
            BandType current = GameManager.Instance.LevelManager.CurrentBand.Type;

            foreach (BandAmount entry in m_BandAmounts)
            {
                if (entry.Band == current)
                    return entry.Amount;
            }

            return 0;
        }

        public override void PlayerEntered(PlayerController player)
        {
            m_Board.ClearCell(m_Cell);

            AudioManager.Instance.PlayFoodConsumedSFX();

            ApplyEffect(player);

            Destroy(gameObject);
        }
    }
}
