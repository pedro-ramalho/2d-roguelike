using UnityEngine;

public class TurnManager
{
    private int m_TurnCount;

    public TurnManager()
    {
        m_TurnCount = 1;
    }

    public void Tick()
    {
        Debug.Log($"Turn {m_TurnCount}");
        m_TurnCount++;
    }
}
