using System.Collections;
using UnityEngine;

public class Ranged : EnemyController
{
    private bool m_IsOnCooldown;
    private static readonly int m_AttackRange = 2;

    private static readonly int m_RetreatDistance = 2;
    private static readonly int m_RunwayScanDepth = 2;

    void Awake() => m_IsOnCooldown = false;

    bool IsWithinRange(Vector2Int delta) => Mathf.Abs(delta.x) + Mathf.Abs(delta.y) <= m_AttackRange;

    void HandleWithinLineOfSightAction(Vector2Int delta, bool canAttack)
    {
        if (IsWithinRange(delta) && canAttack)
        {
            AttackPlayer(delta);
            m_IsOnCooldown = true;
        }
        else
            MoveTowards(delta);
    }

    int CountFreeCellsInDirection(Vector2Int direction)
    {
        int count = 0;
        int freeCells = 0;

        Vector2Int cursor = Cell;

        while (count < m_RunwayScanDepth)
        {
            cursor += direction;

            if (!CanEnterCell(cursor))
                break;
            
            freeCells++;

            count++;
        }

        return freeCells;
    }

    Vector2Int EvaluateRetreatDirection(Vector2Int orientation)
    {
        if (orientation == Vector2Int.down)
        {
            int freeCellsLeft = CountFreeCellsInDirection(Vector2Int.left);
            int freeCellsUp = CountFreeCellsInDirection(Vector2Int.up);
            int freeCellsRight = CountFreeCellsInDirection(Vector2Int.right);

            int maxFreeCells = Mathf.Max(freeCellsLeft, freeCellsUp, freeCellsRight);
            
            if (maxFreeCells == 0) return Vector2Int.zero;
            if (maxFreeCells == freeCellsLeft) return Vector2Int.left;
            if (maxFreeCells == freeCellsUp) return Vector2Int.up;
            if (maxFreeCells == freeCellsRight) return Vector2Int.right;
        }
        else if (orientation == Vector2Int.left)
        {
            int freeCellsUp = CountFreeCellsInDirection(Vector2Int.up);
            int freeCellsRight = CountFreeCellsInDirection(Vector2Int.right);
            int freeCellsDown = CountFreeCellsInDirection(Vector2Int.down);

            int maxFreeCells = Mathf.Max(freeCellsUp, freeCellsRight, freeCellsDown);

            if (maxFreeCells == 0) return Vector2Int.zero;
            if (maxFreeCells == freeCellsUp) return Vector2Int.up;
            if (maxFreeCells == freeCellsRight) return Vector2Int.right;
            if (maxFreeCells == freeCellsDown) return Vector2Int.down;
        }
        else if (orientation == Vector2Int.up)
        {
            int freeCellsRight = CountFreeCellsInDirection(Vector2Int.right);
            int freeCellsDown = CountFreeCellsInDirection(Vector2Int.down);
            int freeCellsLeft = CountFreeCellsInDirection(Vector2Int.left);

            int maxFreeCells = Mathf.Max(freeCellsRight, freeCellsDown, freeCellsLeft);

            if (maxFreeCells == 0) return Vector2Int.zero;
            if (maxFreeCells == freeCellsRight) return Vector2Int.right;
            if (maxFreeCells == freeCellsDown) return Vector2Int.down;
            if (maxFreeCells == freeCellsLeft) return Vector2Int.left;
        }
        else
        {
            int freeCellsDown = CountFreeCellsInDirection(Vector2Int.down);
            int freeCellsLeft = CountFreeCellsInDirection(Vector2Int.left);
            int freeCellsUp = CountFreeCellsInDirection(Vector2Int.up);

            int maxFreeCells = Mathf.Max(freeCellsDown, freeCellsLeft, freeCellsUp);

            if (maxFreeCells == 0) return Vector2Int.zero;
            if (maxFreeCells == freeCellsDown) return Vector2Int.down;
            if (maxFreeCells == freeCellsLeft) return Vector2Int.left;
            if (maxFreeCells == freeCellsUp) return Vector2Int.up;
        }

        return Vector2Int.zero;
    }

    IEnumerator RetreatActionCoroutine()
    {  
        // This works because we only call this routine when the Enemy is adjacent
        Vector2Int orientation = ComputeDeltaToPlayer();
        Vector2Int direction = EvaluateRetreatDirection(orientation);

        if (direction == Vector2Int.zero)
            yield break;

        for (int i = 0; i < m_RetreatDistance; i++)
        {
            if (TryMove(direction))
                yield return new WaitUntil(() => !Animator.IsBusy);
        }
    }

    void HandleRetreatAction() => StartCoroutine(RetreatActionCoroutine());

    protected override void ResolveEnemyAction()
    {
        bool couldAttackThisTurn = !m_IsOnCooldown;
        m_IsOnCooldown = false;

        Vector2Int delta = ComputeDeltaToPlayer();

        if (IsAdjacentToPlayer(delta))
        {
            HandleRetreatAction();
        }
        else if (IsInLineOfSightToPlayer())
        {
            HandleWithinLineOfSightAction(delta, couldAttackThisTurn);
        }
        else
        {
            MoveTowards(delta);
        }
    }
}
