using UnityEngine;

public class Tank : EnemyController
{
    [SerializeField]
    private int m_FirstAllowedLevel = 1;

    protected override void ResolveEnemyAction()
    {
        // TODO: link block gain with the current level
        Combatant.AddBlock(3);

        Vector2Int delta = ComputeDeltaToPlayer();
        ChaseOrAttack(delta);
    }
}
