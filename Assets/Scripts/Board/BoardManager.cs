using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool Passable;
        public ICellOccupant ContainedObject;    
    }

    private Tilemap m_Tilemap;
    private CellData[,] m_BoardData;
    private Grid m_Grid;

    private int m_Width;
    private int m_Height;

    public int Width => m_Width;
    public int Height => m_Height;

    public void Init(int width, int height)
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();

        m_Width = width;
        m_Height = height;

        m_BoardData = new CellData[m_Width, m_Height];

        for (int y = 0; y < m_Height; y++)
            for (int x = 0; x < m_Width; x++)
                m_BoardData[x, y] = new CellData();
    }

    public void AddObject(CellObject obj, Vector2Int coord)
    {
        CellData data = m_BoardData[coord.x, coord.y];

        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;

        obj.Init(this, coord);
    }

    public Vector3 CellToWorld(Vector2Int cellIndex) => m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    
    public CellData GetCellData(Vector2Int cellIndex)
    {
        bool isOutsideBoardX = cellIndex.x < 0 || cellIndex.x >= m_Width;
        bool isOutsideBoardY = cellIndex.y < 0 || cellIndex.y >= m_Height;
        bool isOutsideBoard = isOutsideBoardX || isOutsideBoardY;

        if (isOutsideBoard)
            return null;
        
        return m_BoardData[cellIndex.x, cellIndex.y];
    }

    public void ClearCell(Vector2Int cellIndex)
    {
        CellData cellData = GetCellData(cellIndex);

        if (cellData != null)
            cellData.ContainedObject = null;
    }

    public void SetCellOccupant(Vector2Int cellIndex, ICellOccupant occupant) => m_BoardData[cellIndex.x, cellIndex.y].ContainedObject = occupant;

    public void SetCellPassable(Vector2Int cellIndex, bool passable) => GetCellData(cellIndex).Passable = passable;
    
    public void SetCellTile(Vector2Int cellIndex, Tile tile) => m_Tilemap.SetTile((Vector3Int)cellIndex, tile);

    public Tile GetCellTile(Vector2Int cellIndex) => m_Tilemap.GetTile<Tile>((Vector3Int)cellIndex);
    
    public void Clean()
    {
        if (m_BoardData == null)
            return;
        
        for (int y = 0; y < m_Height; y++)
            for (int x = 0; x < m_Width; x++)
            {
                CellData cellData = m_BoardData[x, y];

                if (cellData.ContainedObject != null)
                    Destroy(cellData.ContainedObject.GameObject);

                SetCellTile(new Vector2Int(x, y), null);
            }
    }
}
