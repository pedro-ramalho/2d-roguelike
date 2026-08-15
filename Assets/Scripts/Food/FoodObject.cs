using UnityEngine;

public abstract class FoodObject : CellObject
{
    [SerializeField]
    protected AudioClip m_ConsumeSFX;

    protected abstract void ApplyEffect(PlayerController player);

    public override void PlayerEntered(PlayerController player)
    {
        m_Board.ClearCell(m_Cell);

        ApplyEffect(player);

        Destroy(gameObject);
    }
}
