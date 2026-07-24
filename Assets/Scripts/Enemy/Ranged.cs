using UnityEngine;

public class Ranged : EnemyController
{
    private bool m_IsOnCooldown;
    private readonly int m_AttackRange = 2;

    void Awake() => m_IsOnCooldown = false;

    void HandleChaseAction()
    {
        
    }

    void HandleWithinLineOfSightAction()
    {
        
    }

    void HandleRetreatAction()
    {
        
    }

    protected override void ResolveEnemyAction()
    {
        Vector2Int delta = ComputeDeltaToPlayer();

        if (IsAdjacentToPlayer(delta))
        {
            // Retreat
        }
        else if (IsInLineOfSightToPlayer())
        {
            // If within range
                // If NOT on cooldown, Attack
                // Else, sit still
            // Else, chase Player
        }
        else
        {
            // Chase Player
        }
    }
}
