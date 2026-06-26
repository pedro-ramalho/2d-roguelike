using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool Passable;
        public CellObject ContainedObject;    
    }

    private Tilemap m_Tilemap;
    private CellData[,] m_BoardData;
    private Grid m_Grid;
    private List<Vector2Int> m_EmptyCells;

    public int Width;
    public int Height;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;
    public PlayerController Player;
    public FoodObject[] FoodPrefabs;

    public void Init()
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();
        m_BoardData = new CellData[Width, Height];
        m_EmptyCells = new List<Vector2Int>();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Tile tile;
                m_BoardData[x, y] = new CellData();

                bool isBorder = x == 0 || y == 0 || x == Width - 1 || y == Height - 1;
                if (isBorder)
                {
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];
                    
                    m_BoardData[x, y].Passable = false;
                }
                else
                {
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];

                    m_BoardData[x, y].Passable = true;
                    m_EmptyCells.Add(new Vector2Int(x, y));
                }

                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        m_EmptyCells.Remove(new Vector2Int(1, 1));
        GenerateFood();
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width || cellIndex.y < 0 || cellIndex.y >= Height)
        {
            return null;
        }

        return m_BoardData[cellIndex.x, cellIndex.y];
    }

    void GenerateFood()
    {
        int foodCount = Random.Range(2, 6);
        for (int i = 0; i < foodCount; i++)
        {
            int randomCellIndex = Random.Range(0, m_EmptyCells.Count);
            Vector2Int coord = m_EmptyCells[randomCellIndex];

            m_EmptyCells.RemoveAt(randomCellIndex);
            CellData data = m_BoardData[coord.x, coord.y];

            int randomFoodIndex = Random.Range(0, FoodPrefabs.Length);
            FoodObject newFood = Instantiate(FoodPrefabs[randomFoodIndex]);
            newFood.transform.position = CellToWorld(coord);
            data.ContainedObject = newFood;
        }
    }
}
