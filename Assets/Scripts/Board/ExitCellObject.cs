using Core;
using Player;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Board
{
    public class ExitCellObject : CellObject
    {
        public Tile endTile;

        protected override void Awake()
        {
            base.Awake();

            m_Board.SetCellTile(m_Cell, endTile);
        }

        public override void PlayerEntered(PlayerController _)
        {
            m_Board.ClearCell(m_Cell);
            GameManager.Instance?.LevelManager?.NewLevel();
        }
    }
}
