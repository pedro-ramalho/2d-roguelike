using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private int m_TurnCount;

    public event System.Action OnTick;

    public TurnManager()
    {
        m_TurnCount = 1;
    }

    public void Tick()
    {
        Debug.Log($"Turn {m_TurnCount}");
        m_TurnCount++;
        OnTick?.Invoke();
    }
}
