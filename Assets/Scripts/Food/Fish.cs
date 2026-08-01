using UnityEngine;

public class Fish : FoodObject
{
    [SerializeField]
    private int m_BlockPoints = 3;

    [SerializeField]
    private int m_FirstAllowedLevel = 1;

    protected override void ApplyEffect(PlayerController player) =>
        player.Combatant.AddBlock(m_BlockPoints);
}
