using Player;
using UnityEngine;

namespace Board
{
    public class CellObject : MonoBehaviour, ICellOccupant
    {
        protected BoardManager m_Board;
        protected Vector2Int m_Cell;

        public GameObject GameObject => gameObject;

        protected virtual void Awake()
        {
            m_Board = Core.GameManager.Instance.BoardManager;
            m_Cell = m_Board.WorldToCell(transform.position);
        }

        public virtual void PlayerEntered(PlayerController player) { }

        public virtual bool PlayerWantsToEnter() => true;
    }
}
