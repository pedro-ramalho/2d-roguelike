using UnityEngine;

public class FoodObject : CellObject
{
    public int Points = 5;
    
    public override void PlayerEntered(PlayerController player)
    {
        GameManager.Instance.BoardManager.ClearCell(m_Cell);
        Destroy(gameObject);

        player.PlayerStats.ChangeStamina(Points);
    }
}
