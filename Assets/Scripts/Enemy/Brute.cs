using UnityEngine;

public class Brute : EnemyController
{
    protected override void ResolveEnemyAction()
    {
        Vector2Int delta = ComputeDeltaToPlayer();

        if (IsAdjacentToPlayer(delta))
            AttackPlayer(delta);
        else
            MoveTowards(delta);        
    }
}
