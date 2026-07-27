using UnityEngine;

public class CellObject : MonoBehaviour, ICellOccupant
{
    protected BoardManager m_Board;
    protected Vector2Int m_Cell;

    public GameObject GameObject => gameObject;

    public virtual void Init(BoardManager board, Vector2Int cell)
    {
        m_Board = board; 
        m_Cell = cell;
    }
    
    public virtual void PlayerEntered(PlayerController player) {}

    public virtual bool PlayerWantsToEnter() => true;
}
