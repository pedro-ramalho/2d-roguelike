using UnityEngine;
using UnityEngine.Tilemaps;

public class ExitCellObject : CellObject
{
    public Tile endTile;

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);

        GameManager.Instance?.BoardManager?.SetCellTile(coord, endTile);
    }

    public override void PlayerEntered(PlayerController _) => GameManager.Instance?.NewLevel();
}
