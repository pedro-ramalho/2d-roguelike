using UnityEngine;

public class CellObject : MonoBehaviour
{
    protected Vector2Int m_Cell;

    public virtual void Init(Vector2Int cell)
    {
        m_Cell = cell;
    }

    public virtual void PlayerEntered(Player player) {}

    public virtual bool PlayerWantsToEnter() => true;
}
