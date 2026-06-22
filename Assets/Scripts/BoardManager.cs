using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    private Tilemap m_Tilemap;

    public int Width;
    public int Height;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;

    void Start()
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Tile tile;

                bool isBorder = x == 0 || y == 0 || x == Width - 1 || y == Height - 1;
                if (isBorder)
                {
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];
                }
                else
                {
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                }

                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }
}
