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

    // Dimensions
    [SerializeField] private int m_Width;
    [SerializeField] private int m_Height;

    // Private references
    private BoardManager m_BoardManager;
    private TurnManager m_TurnManager;
    private PlayerController m_PlayerController;
    
    void GenerateWall()
    {
        
    }

    void GenerateFood()
    {
        
    }

    void GenerateEnemy()
    {
        
    }

    public void GenerateBoard(BoardManager boardManager, TurnManager turnManager, PlayerController playerController)
    {
        m_BoardManager = boardManager;
        m_TurnManager = turnManager;
        m_PlayerController = playerController;
    }
}
