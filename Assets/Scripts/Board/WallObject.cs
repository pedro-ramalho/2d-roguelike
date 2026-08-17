using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObject : CellObject
{
    private int m_HealthPoint;
    private Tile m_OriginalTile;

    public Tile[] ObstacleTiles;
    public int MaxHealth = 2;

    Tile GetTileByHealthPoint()
    {
        if (m_HealthPoint <= 0)
            return null;

        return ObstacleTiles[MaxHealth - m_HealthPoint];
    }

    public override void Init(BoardManager board, Vector2Int cell)
    {
        base.Init(board, cell);

        m_HealthPoint = MaxHealth;

        m_OriginalTile = m_Board.GetCellTile(cell);
        m_Board.SetCellTile(cell, GetTileByHealthPoint());
    }

    public override bool PlayerWantsToEnter()
    {
        m_HealthPoint--;

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
