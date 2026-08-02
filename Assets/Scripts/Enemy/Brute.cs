using UnityEngine;

public class Brute : EnemyController
{
    [SerializeField]
    private int m_FirstAllowedLevel = 1;

    protected override void ResolveEnemyAction()
    {
        Vector2Int delta = ComputeDeltaToPlayer();
        ChaseOrAttack(delta);
    }
}
