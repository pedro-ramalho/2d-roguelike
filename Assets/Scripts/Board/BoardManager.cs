using UnityEngine;
using UnityEngine.Tilemaps;

namespace Board
{
    public class BoardManager : MonoBehaviour
    {
        public class CellData
        {
            public bool Passable;
            public ICellOccupant ContainedObject;
        }

        [SerializeField]
        private Grid m_Grid;

        [SerializeField]
        private Tilemap m_Tilemap;

        private CellData[,] m_BoardData;

        private int m_Width;
        private int m_Height;

        public int Width => m_Width;
        public int Height => m_Height;

        private Vector2Int ComputeStepVector(Vector2Int delta)
        {
            if (delta.x > 0)
                return new Vector2Int(1, 0);
            if (delta.x < 0)
                return new Vector2Int(-1, 0);
            if (delta.y > 0)
                return new Vector2Int(0, 1);
            if (delta.y < 0)
                return new Vector2Int(0, -1);

            return new Vector2Int(0, 0);
        }

        public void Init(int width, int height)
        {
            m_Width = width;
            m_Height = height;

            m_BoardData = new CellData[m_Width, m_Height];

            for (int y = 0; y < m_Height; y++)
            for (int x = 0; x < m_Width; x++)
                m_BoardData[x, y] = new CellData();
        }

        public T Place<T>(T prefab, Vector2Int cell)
            where T : Component, ICellOccupant
        {
            T instance = Instantiate(prefab, CellToWorld(cell), Quaternion.identity);
            m_BoardData[cell.x, cell.y].ContainedObject = instance;

            return instance;
        }

        public Vector3 CellToWorld(Vector2Int cellIndex) =>
            m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);

        public Vector2Int WorldToCell(Vector3 worldPosition) =>
            (Vector2Int)m_Grid.WorldToCell(worldPosition);

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

        public void SetCellOccupant(Vector2Int cellIndex, ICellOccupant occupant) =>
            m_BoardData[cellIndex.x, cellIndex.y].ContainedObject = occupant;

        public void SetCellPassable(Vector2Int cellIndex, bool passable) =>
            GetCellData(cellIndex).Passable = passable;

        public void SetCellTile(Vector2Int cellIndex, Tile tile) =>
            m_Tilemap.SetTile((Vector3Int)cellIndex, tile);

        public Tile GetCellTile(Vector2Int cellIndex) =>
            m_Tilemap.GetTile<Tile>((Vector3Int)cellIndex);

        public bool IsCellFree(Vector2Int cellIndex) =>
            GetCellData(cellIndex).ContainedObject == null;

        public bool IsInLineOfSight(Vector2Int coordA, Vector2Int coordB)
        {
            Vector2Int delta = coordB - coordA;

            if (delta.x != 0 && delta.y != 0)
                return false;

            Vector2Int step = ComputeStepVector(delta);

            Vector2Int cursor = coordA;
            while (true)
            {
                cursor += step;

                if (cursor == coordB)
                    break;

                if (!IsCellFree(cursor))
                    return false;
            }

            return true;
        }

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
}
