using System.Collections.Generic;
using Core;
using Enemy;
using Food;
using Level;
using Player;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Board
{
    public class BoardGenerator : MonoBehaviour
    {
        // Tiles
        [SerializeField]
        private Tile[] m_GroundTiles;

        [SerializeField]
        private Tile[] m_WallTiles;

        // Prefabs
        [SerializeField]
        private WallObject m_WallPrefab;

        [SerializeField]
        private ExitCellObject m_ExitPrefab;

        // Private references
        private BoardManager m_BoardManager;
        private PlayerController m_PlayerController;
        private LevelBand m_CurrentBand;

        // Empty cells
        private List<Vector2Int> m_EmptyCells;

        // Level
        private int m_CurrentLevel;

        Vector2Int GetRandomEmptyCell()
        {
            if (m_EmptyCells.Count == 0)
                return new Vector2Int(-1, -1);

            int index = Random.Range(0, m_EmptyCells.Count);
            Vector2Int coord = m_EmptyCells[index];
            m_EmptyCells.RemoveAt(index);

            return coord;
        }

        void GenerateWalls()
        {
            int wallCount = Random.Range(
                m_CurrentBand.MinWallCount,
                m_CurrentBand.MaxWallCount + 1
            );

            for (int i = 0; i < wallCount; i++)
            {
                Vector2Int cell = GetRandomEmptyCell();
                WallObject wall = Instantiate(m_WallPrefab);

                m_BoardManager.AddObject(wall, cell);
            }
        }

        void GenerateFood()
        {
            foreach (var entry in m_CurrentBand.FoodEntries)
            {
                int count = Random.Range(entry.MinCount, entry.MaxCount + 1);

                for (int i = 0; i < count; i++)
                {
                    Vector2Int cell = GetRandomEmptyCell();
                    FoodObject food = Instantiate(entry.Prefab);

                    m_BoardManager.AddObject(food, cell);
                }
            }
        }

        bool IsEliteLevel(int level) =>
            level % GameManager.Instance.ProgressionSettings.EliteCadence == 0;

        EnemyController SpawnEnemy(EnemyController prefab)
        {
            if (m_EmptyCells.Count == 0)
                return null;

            Vector2Int cell = GetRandomEmptyCell();

            EnemyController enemy = Instantiate(prefab);
            m_BoardManager.SetCellOccupant(cell, enemy.Combatant);

            enemy.Spawn(m_BoardManager, m_PlayerController, cell);
            enemy.Combatant.ApplyBandStats();

            return enemy;
        }

        void GenerateEnemies()
        {
            List<EnemyController> spawned = new List<EnemyController>();

            foreach (var entry in m_CurrentBand.EnemyEntries)
            {
                int count = Random.Range(entry.MinCount, entry.MaxCount + 1);

                for (int i = 0; i < count; i++)
                {
                    EnemyController e = SpawnEnemy(entry.Prefab);
                    if (e != null)
                        spawned.Add(e);
                }
            }

            if (IsEliteLevel(m_CurrentLevel) && spawned.Count > 0)
            {
                EnemyController chosen = spawned[Random.Range(0, spawned.Count)];
                chosen.gameObject.AddComponent<EliteModifier>();
            }
        }

        public void GenerateBoard(
            BoardManager boardManager,
            PlayerController playerController,
            int currentLevel
        )
        {
            m_BoardManager = boardManager;
            m_PlayerController = playerController;
            m_CurrentBand = GameManager.Instance.LevelManager.CurrentBand;
            m_CurrentLevel = currentLevel;

            m_EmptyCells = new List<Vector2Int>();

            for (int y = 0; y < boardManager.Height; y++)
            {
                for (int x = 0; x < boardManager.Width; x++)
                {
                    Tile tile;
                    Vector2Int coord = new Vector2Int(x, y);

                    bool isBorder =
                        x == 0
                        || y == 0
                        || x == boardManager.Width - 1
                        || y == boardManager.Height - 1;
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

            GenerateWalls();
            GenerateFood();
            GenerateEnemies();
        }
    }
}
