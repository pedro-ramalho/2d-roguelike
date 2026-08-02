using UnityEngine;

public abstract class FoodObject : CellObject
{
    [SerializeField]
    private int m_FirstAllowedLevel = 1;

    public int FirstAllowedLevel => m_FirstAllowedLevel;

    protected abstract void ApplyEffect(PlayerController player);

    public override void PlayerEntered(PlayerController player)
    {
        m_Board.ClearCell(m_Cell);

        ApplyEffect(player);

        Destroy(gameObject);
    }
}
