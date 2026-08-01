using UnityEngine;
using UnityEngine.Tilemaps;

public class ExitCellObject : CellObject
{
    public Tile endTile;

    public override void Init(BoardManager board, Vector2Int coord)
    {
        base.Init(board, coord);

        m_Board.SetCellTile(coord, endTile);
    }

    public override void PlayerEntered(PlayerController _) =>
        GameManager.Instance?.LevelManager?.NewLevel();
}
