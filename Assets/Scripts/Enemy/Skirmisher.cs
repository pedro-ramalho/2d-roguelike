using System.Collections;
using UnityEngine;

public class Skirmisher : EnemyController
{
    private static readonly int m_SkirmisherTurns = 2;

    IEnumerator ResolveSkirmisherAction()
    {
        for (int i = 0; i < m_SkirmisherTurns; i++)
        {
            Vector2Int delta = ComputeDeltaToPlayer();
            if (IsAdjacentToPlayer(delta))
            {
                AttackPlayer(delta);

                break;
            }
            else
            {
                MoveTowards(delta);

                yield return new WaitUntil(() => !Animator.IsBusy);
            }
        }
    }

    protected override void ResolveEnemyAction() => StartCoroutine(ResolveSkirmisherAction());
}
