using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private BoardManager m_Board;
    private Vector2Int m_CellPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_Board = boardManager;
        m_CellPosition = cell;

        transform.position = m_Board.CellToWorld(cell);
    }
}
