using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 3f;

    private BoardManager m_Board;
    private Vector2Int m_CellPosition;
    private bool m_IsGameOver;
    private Animator m_Animator;
    private bool m_IsMoving;
    private Vector3 m_MoveTarget;

    private void Awake() => m_Animator = GetComponent<Animator>();

    public void Init() => m_IsGameOver = false;

    // Update is called once per frame
    void Update()
    {
        if (m_IsGameOver)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                GameManager.Instance.StartNewGame();    
            }

            return;
        }

        Vector2Int newCellTarget = m_CellPosition;
        bool hasMoved = false;

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            newCellTarget += Vector2Int.up;
            hasMoved = true;
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            newCellTarget += Vector2Int.down;
            hasMoved = true;
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            newCellTarget += Vector2Int.left;
            hasMoved = true;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            newCellTarget += Vector2Int.right;
            hasMoved = true;
        }

        if (m_IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_MoveTarget, MoveSpeed * Time.deltaTime);

            if (transform.position == m_MoveTarget)
            {
                m_IsMoving = false;
                m_Animator.SetBool("Moving", false);

                var cellData = m_Board.GetCellData(m_CellPosition);

                if (cellData.ContainedObject != null)
                {
                    cellData.ContainedObject.PlayerEntered();
                }
            }

            return;
        }

        if (hasMoved)
        {
            BoardManager.CellData cellData = m_Board.GetCellData(newCellTarget);
            if (cellData != null && cellData.Passable)
            {
                GameManager.Instance.TurnManager.Tick();
                if (cellData.ContainedObject == null)
                {
                    MoveTo(newCellTarget);
                }
                else if (cellData.ContainedObject.PlayerWantsToEnter())
                {
                    MoveTo(newCellTarget);
                    cellData.ContainedObject.PlayerEntered();
                }
                else
                {
                    m_Animator.SetTrigger("Attack");
                }
            }
        }
    }

    public void GameOver() => m_IsGameOver = true;

    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_Board = boardManager;
        MoveTo(cell, true);
    }

    public void MoveTo(Vector2Int cell, bool immediate = false)
    {
        m_CellPosition = cell;

        if (immediate)
        {
            m_IsMoving = false;
            transform.position = m_Board.CellToWorld(m_CellPosition);    
        }
        else
        {
            m_IsMoving = true;
            m_MoveTarget = m_Board.CellToWorld(m_CellPosition);
        }

        m_Animator.SetBool("Moving", m_IsMoving);
    }
}
