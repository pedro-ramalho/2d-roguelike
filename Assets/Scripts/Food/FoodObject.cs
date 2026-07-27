using UnityEngine;

public abstract class FoodObject : CellObject
{    
    protected abstract void ApplyEffect(PlayerController player);

    public override void PlayerEntered(PlayerController player)
    {
        m_Board.ClearCell(m_Cell);
        
        Destroy(gameObject);

        ApplyEffect(player);
    }
}
