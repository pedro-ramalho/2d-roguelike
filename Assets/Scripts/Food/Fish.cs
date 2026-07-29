using UnityEngine;

public class Fish : FoodObject
{
    [SerializeField] private int m_BlockPoints = 3;

    protected override void ApplyEffect(PlayerController player) => player.Combatant.AddBlock(m_BlockPoints);
}
