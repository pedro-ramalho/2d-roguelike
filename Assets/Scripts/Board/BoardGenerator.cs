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
        [SerializeField]
        private Tile[] m_GroundTiles;

        [SerializeField]
        private Tile[] m_WallTiles;

        [SerializeField]
        private WallObject m_WallPrefab;

        [SerializeField]
        private ExitCellObject m_ExitPrefab;

        private BoardManager m_BoardManager;
        private LevelBand m_CurrentBand;

        private List<Vector2Int> m_EmptyCells;

        void Setup(BoardManager board)
        {
            m_BoardManager = board;
            m_CurrentBand = GameManager.Instance.LevelManager.CurrentBand;
            m_EmptyCells = new List<Vector2Int>();
        }

        void CreateBorderTile(Vector2Int coord)
        {
            Tile tile = m_WallTiles[Random.Range(0, m_WallTiles.Length)];
            m_BoardManager.SetCellTile(coord, tile);
            m_BoardManager.SetCellPassable(coord, false);
        }

        void CreateGroundTile(Vector2Int coord)
        {
            Tile tile = m_GroundTiles[Random.Range(0, m_GroundTiles.Length)];
            m_BoardManager.SetCellTile(coord, tile);
            m_BoardManager.SetCellPassable(coord, true);
            m_EmptyCells.Add(coord);
        }

        void CreateTile(int x, int y)
        {
            Vector2Int coord = new Vector2Int(x, y);

            if (IsBorder(x, y))
                CreateBorderTile(coord);
            else
                CreateGroundTile(coord);
        }

        void CreateTiles()
        {
            for (int y = 0; y < m_BoardManager.Height; y++)
            for (int x = 0; x < m_BoardManager.Width; x++)
                CreateTile(x, y);
        }

        void ReserveSpecialCells()
        {
            m_EmptyCells.Remove(new Vector2Int(1, 1));
            m_EmptyCells.Remove(
                new Vector2Int(m_BoardManager.Width - 2, m_BoardManager.Height - 2)
            );
        }

        bool TryGetRandomEmptyCell(out Vector2Int coord)
        {
            if (m_EmptyCells.Count == 0)
            {
                coord = new Vector2Int(-1, -1);
                return false;
            }

            int index = Random.Range(0, m_EmptyCells.Count);
            coord = m_EmptyCells[index];
            m_EmptyCells.RemoveAt(index);

            return true;
        }

        void GenerateExitCell()
        {
            Vector2Int endCoord = new Vector2Int(
                m_BoardManager.Width - 2,
                m_BoardManager.Height - 2
            );
            m_BoardManager.Place(m_ExitPrefab, endCoord);
        }

        void GenerateWalls()
        {
            int count = Random.Range(m_CurrentBand.MinWallCount, m_CurrentBand.MaxWallCount + 1);

            for (int i = 0; i < count; i++)
            {
                if (TryGetRandomEmptyCell(out Vector2Int coord))
                    m_BoardManager.Place(m_WallPrefab, coord);
            }
        }

        void GenerateFood()
        {
            foreach (var entry in m_CurrentBand.FoodEntries)
            {
                int count = Random.Range(entry.MinCount, entry.MaxCount + 1);

                for (int i = 0; i < count; i++)
                    if (TryGetRandomEmptyCell(out Vector2Int coord))
                        m_BoardManager.Place(entry.Prefab, coord);
            }
        }

        EnemyController SpawnEnemy(EnemyController prefab)
        {
            if (!TryGetRandomEmptyCell(out Vector2Int coord))
                return null;

            EnemyController enemy = m_BoardManager.Place(prefab, coord);
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

            if (GameManager.Instance.LevelManager.IsEliteLevel() && spawned.Count > 0)
            {
                EnemyController chosen = spawned[Random.Range(0, spawned.Count)];

                chosen.gameObject.AddComponent<EliteModifier>();
            }
        }

        bool IsBorder(int x, int y) =>
            x == 0 || y == 0 || x == m_BoardManager.Width - 1 || y == m_BoardManager.Height - 1;

        public void GenerateBoard(BoardManager board)
        {
            Setup(board);

            CreateTiles();
            ReserveSpecialCells();

            GenerateExitCell();
            GenerateWalls();
            GenerateFood();
            GenerateEnemies();
        }
    }
}
