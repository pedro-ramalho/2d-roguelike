using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardGenerator : MonoBehaviour
{
    // Tiles
    [SerializeField] private Tile[] m_GroundTiles;
    [SerializeField] private Tile[] m_WallTiles;

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

    // Private references
    private BoardManager m_BoardManager;
    private TurnManager m_TurnManager;
    private PlayerController m_PlayerController;

    // Empty cells
    private List<Vector2Int> m_EmptyCells;

    // Level
    private int m_CurrentLevel;

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

    bool IsEliteLevel(int level) => level % 5 == 0;

    void GenerateEnemy()
    {
        int randomIndex = Random.Range(0, m_EmptyCells.Count);
        Vector2Int coord = m_EmptyCells[randomIndex];

        m_EmptyCells.RemoveAt(randomIndex);
        Combatant newEnemy = Instantiate(m_EnemyPrefab);

        m_BoardManager.SetCellOccupant(coord, newEnemy);

        Tank controller = newEnemy.GetComponent<Tank>();
        if (IsEliteLevel(m_CurrentLevel) && Random.value < 0.25f)
            controller.gameObject.AddComponent<EliteModifier>();
            
        controller.Spawn(m_BoardManager, m_TurnManager, m_PlayerController, coord, m_CurrentLevel);
    }

    public void GenerateBoard(BoardManager boardManager, TurnManager turnManager, PlayerController playerController, int currentLevel)
    {
        m_BoardManager = boardManager;
        m_TurnManager = turnManager;
        m_PlayerController = playerController;
        m_CurrentLevel = currentLevel;

        m_EmptyCells = new List<Vector2Int>();

        for (int y = 0; y < boardManager.Height; y++)
        {
            for (int x = 0; x < boardManager.Width; x++)
            {
                Tile tile;
                Vector2Int coord = new Vector2Int(x, y);

                bool isBorder = x == 0 || y == 0 || x == boardManager.Width - 1 || y == boardManager.Height - 1;
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

                m_BoardManager.SetCellTile(coord, tile);
            }
        }

        m_EmptyCells.Remove(new Vector2Int(1, 1));

        Vector2Int endCoord = new Vector2Int(boardManager.Width - 2, boardManager.Height - 2);
        m_BoardManager.AddObject(Instantiate(m_ExitPrefab), endCoord);
        m_EmptyCells.Remove(endCoord);

        GenerateWall();
        GenerateFood();
        GenerateEnemy();
    }
}
