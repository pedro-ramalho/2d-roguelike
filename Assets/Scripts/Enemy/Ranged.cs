using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class Ranged : EnemyController
    {
        [SerializeField]
        private Projectile.Projectile m_ProjectilePrefab;

        private bool m_IsOnCooldown;

        private static readonly Vector2Int[] m_Cardinals =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
        };

        private static readonly int m_AttackRange = 4;
        private static readonly int m_RetreatDistance = 2;
        private static readonly int m_RunwayScanDepth = 2;

        bool IsWithinRange(Vector2Int delta) =>
            Mathf.Abs(delta.x) + Mathf.Abs(delta.y) <= m_AttackRange;

        void HandleWithinLineOfSightAction(Vector2Int delta, bool canAttack)
        {
            if (IsWithinRange(delta) && canAttack)
            {
                Vector2Int targetCell = PlayerCell;

                Animator.PlayProjectileAnimation(
                    m_ProjectilePrefab,
                    PlayerCellToWorld(),
                    () =>
                    {
                        if (PlayerCell == targetCell)
                            DealDamageToPlayer();
                    }
                );

                m_IsOnCooldown = true;
            }
            else
                MoveTowards(delta);
        }

        int CountFreeCellsInDirection(Vector2Int direction)
        {
            int count = 0;
            int freeCells = 0;

            Vector2Int cursor = Combatant.Cell;

            while (count < m_RunwayScanDepth)
            {
                cursor += direction;

                if (!Combatant.CanMoveTo(cursor))
                    break;

                freeCells++;

                count++;
            }

            return freeCells;
        }

        Vector2Int EvaluateRetreatDirection(Vector2Int orientation)
        {
            int bestScore = 0;
            Vector2Int bestDirection = Vector2Int.zero;

            foreach (Vector2Int cardinal in m_Cardinals)
            {
                if (cardinal == orientation)
                    continue;

                int score = CountFreeCellsInDirection(cardinal);
                if (score > bestScore)
                {
                    bestDirection = cardinal;
                    bestScore = score;
                }
            }

            return bestDirection;
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
                if (Combatant.TryMoveTo(Combatant.Cell + direction))
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
}
