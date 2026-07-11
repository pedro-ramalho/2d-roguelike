using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObject : CellObject
{
    private int m_HealthPoint;
    private Tile m_OriginalTile;

    public Tile[] ObstacleTiles;
    public int MaxHealth = 3;
    
    Tile GetTileByHealthPoint()
    {
        if (m_HealthPoint <= 0)
            return null;
        
        return ObstacleTiles[MaxHealth - m_HealthPoint];
    }

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);

        m_HealthPoint = MaxHealth;
        m_OriginalTile = GameManager.Instance.BoardManager.GetCellTile(cell);

        GameManager.Instance.BoardManager.SetCellTile(cell, GetTileByHealthPoint());
    }

    public override bool PlayerWantsToEnter()
    {
        m_HealthPoint--;

        if (m_HealthPoint > 0)
        {
            GameManager.Instance.BoardManager.SetCellTile(m_Cell, GetTileByHealthPoint());
            
            return false;
        }

        GameManager.Instance.BoardManager.SetCellTile(m_Cell, m_OriginalTile);
        Destroy(gameObject);

        return true;
    }
}
