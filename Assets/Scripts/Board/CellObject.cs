using UnityEngine;

public class CellObject : MonoBehaviour, ICellOccupant
{
    protected Vector2Int m_Cell;

    public GameObject GameObject => gameObject;

    public virtual void Init(Vector2Int cell) => m_Cell = cell;
    
    public virtual void PlayerEntered(PlayerController player) {}

    public virtual bool PlayerWantsToEnter() => true;
}
