using UnityEngine;

public abstract class FoodObject : CellObject
{
    [SerializeField]
    private BandAmount[] m_BandAmounts;

    [SerializeField]
    protected AudioClip[] m_ConsumeSFX;

    protected abstract void ApplyEffect(PlayerController player);

    protected int GetAmountForCurrentBand()
    {
        BandType current = GameManager.Instance.LevelManager.CurrentBand.Type;

        foreach (BandAmount entry in m_BandAmounts)
        {
            if (entry.Band == current)
                return entry.Amount;
        }

        return 0;
    }

    public override void PlayerEntered(PlayerController player)
    {
        m_Board.ClearCell(m_Cell);

        AudioManager.Instance.PlayRandomSFXFromList(m_ConsumeSFX);

        ApplyEffect(player);

        Destroy(gameObject);
    }
}
