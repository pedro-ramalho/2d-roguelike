using UnityEngine;

public class Skirmisher : EnemyController
{
    protected override void ResolveEnemyAction()
    {
        for (int attempt = 0; attempt < 2; attempt++)
        {
            Vector2Int delta = ComputeDeltaToPlayer();
            
            if (IsAdjacentToPlayer(delta))
            {
                AttackPlayer(delta);
                break;
            }
            else
                MoveTowards(delta);
        }
    }
}
