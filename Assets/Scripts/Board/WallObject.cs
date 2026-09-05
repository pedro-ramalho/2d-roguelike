using Core;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Board
{
    public class WallObject : CellObject
    {
        [Header("SFX Clips")]
        [SerializeField]
        private AudioClip[] m_BreakSFX;

        [Header("Wall Properties")]
        [SerializeField]
        private Tile[] m_ObstacleTiles;

        [SerializeField]
        private int m_MaxHealth;

        private int m_HealthPoint;
        private Tile m_OriginalTile;

        public Tile[] ObstacleTiles => m_ObstacleTiles;
        public int MaxHealth => m_MaxHealth;

        protected override void Awake()
        {
            base.Awake();

            m_HealthPoint = m_MaxHealth;
            m_OriginalTile = m_Board.GetCellTile(m_Cell);
            m_Board.SetCellTile(m_Cell, GetTileByHealthPoint());
        }

        Tile GetTileByHealthPoint()
        {
            if (m_HealthPoint <= 0)
                return null;

            return ObstacleTiles[MaxHealth - m_HealthPoint];
        }

        public override bool PlayerWantsToEnter()
        {
            m_HealthPoint--;

            if (m_BreakSFX != null && m_BreakSFX.Length > 0)
                AudioManager.Instance.PlayRandomSFXFromList(m_BreakSFX);

            if (m_HealthPoint > 0)
            {
                m_Board.SetCellTile(m_Cell, GetTileByHealthPoint());

                return false;
            }

            m_Board.SetCellTile(m_Cell, m_OriginalTile);
            m_Board.ClearCell(m_Cell);

            Destroy(gameObject);

            return true;
        }
    }
}
