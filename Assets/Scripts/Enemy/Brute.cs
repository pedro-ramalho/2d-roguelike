using UnityEngine;

public class Brute : EnemyController
{
    protected override void ResolveEnemyAction()
    {
        Vector2Int delta = ComputeDeltaToPlayer();
        ChaseOrAttack(delta); 
    }
}
