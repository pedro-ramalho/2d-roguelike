using UnityEngine;

namespace Enemy
{
    public class Tank : EnemyController
    {
        protected override void ResolveEnemyAction()
        {
            Combatant.Stats.AddBlock(3);

            Vector2Int delta = ComputeDeltaToPlayer();
            ChaseOrAttack(delta);
        }
    }
}
