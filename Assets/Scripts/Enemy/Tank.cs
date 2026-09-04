using UnityEngine;

namespace Enemy
{
    public class Tank : EnemyController
    {
        protected override void ResolveEnemyAction()
        {
            // TODO: link block gain with the current level
            Combatant.AddBlock(3);

            Vector2Int delta = ComputeDeltaToPlayer();
            ChaseOrAttack(delta);
        }
    }
}
