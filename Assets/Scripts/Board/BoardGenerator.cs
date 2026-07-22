using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardGenerator : MonoBehaviour
{
    // Tiles
    [SerializeField] private Tile[] m_GroundTiles;
    [SerializeField] private Tile[] m_WallTiles;
    private Tilemap m_Tilemap;

    // Prefabs
    [SerializeField] private FoodObject[] m_FoodPrefabs;
    [SerializeField] private WallObject m_WallPrefab;
    [SerializeField] private ExitCellObject m_ExitPrefab;
    [SerializeField] private Combatant m_EnemyPrefab;

    // Board object counts
    [SerializeField] private int m_MinFoodCount = 2;
    [SerializeField] private int m_MaxFoodCount = 6;
    [SerializeField] private int m_MinWallCount = 6;
    [SerializeField] private int m_MaxWallCount = 10;

    // Dimensions
    private int m_Width;
    private int m_Height;

    // Private references
    private BoardManager m_BoardManager;
    private TurnManager m_TurnManager;
    private PlayerController m_PlayerController;

    // Empty cells
    private List<Vector2Int> m_EmptyCells;

    void GenerateWall()
    {
        int wallCount = Random.Range(m_MinWallCount, m_MaxWallCount);

        for (int i = 0; i < wallCount; i++)
        {
            int randomIndex = Random.Range(0, m_EmptyCells.Count);
            Vector2Int coord = m_EmptyCells[randomIndex];

            m_EmptyCells.RemoveAt(randomIndex);
            WallObject newWall = Instantiate(m_WallPrefab);

            m_BoardManager.AddObject(newWall, coord);
        }
    }

    void GenerateFood()
    {
        int foodCount = Random.Range(m_MinFoodCount, m_MaxFoodCount);

        for (int i = 0; i < foodCount; i++)
        {
            int randomIndex = Random.Range(0, m_EmptyCells.Count);
            Vector2Int coord = m_EmptyCells[randomIndex];

            m_EmptyCells.RemoveAt(randomIndex);

            int randomFoodIndex = Random.Range(0, m_FoodPrefabs.Length);
            FoodObject newFood = Instantiate(m_FoodPrefabs[randomFoodIndex]);

            m_BoardManager.AddObject(newFood, coord);
        }
    }

    void GenerateEnemy()
    {
        int randomIndex = Random.Range(0, m_EmptyCells.Count);
        Vector2Int coord = m_EmptyCells[randomIndex];

        m_EmptyCells.RemoveAt(randomIndex);
        Combatant newEnemy = Instantiate(m_EnemyPrefab);

        m_BoardManager.SetCellOccupant(coord, newEnemy);

        EnemyController controller = newEnemy.GetComponent<EnemyController>();
        controller.Spawn(m_BoardManager, m_TurnManager, m_PlayerController, coord);
    }

    public void GenerateBoard(int width, int height, BoardManager boardManager, TurnManager turnManager, PlayerController playerController)
    {
        m_Width = width;
        m_Height = height;

        m_BoardManager = boardManager;
        m_TurnManager = turnManager;
        m_PlayerController = playerController;

        m_EmptyCells = new List<Vector2Int>();

        for (int y = 0; y < m_Height; y++)
        {
            for (int x = 0; x < m_Width; x++)
            {
                Tile tile;
                Vector2Int coord = new Vector2Int(x, y);

                bool isBorder = x == 0 || y == 0 || x == m_Width - 1 || y == m_Height - 1;
                if (isBorder)
                {   
                    tile = m_WallTiles[Random.Range(0, m_WallTiles.Length)];
                    m_BoardManager.SetCellPassable(coord, false);
                }
                else
                {
                    tile = m_GroundTiles[Random.Range(0, m_GroundTiles.Length)];
                    m_BoardManager.SetCellPassable(coord, true);
                    m_EmptyCells.Add(coord);
                }

                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        m_EmptyCells.Remove(new Vector2Int(1, 1));

        Vector2Int endCoord = new Vector2Int(m_Width - 2, m_Height - 2);
        m_BoardManager.AddObject(Instantiate(m_ExitPrefab), endCoord);
        m_EmptyCells.Remove(endCoord);

        GenerateWall();
        GenerateFood();
        GenerateEnemy();
    }
}
