using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private int m_TurnCount;
    private readonly HashSet<CombatantAnimator> m_Animators = new();

    public event System.Action OnTick;
    public bool IsProcessingTurn { get; private set; }

    void Awake() => m_TurnCount = 1;

    bool AnyBusy() => m_Animators.Any(a => a.IsBusy);

    IEnumerator ProcessTurnCoroutine()
    {
        IsProcessingTurn = true;

        // wait until the Player becomes idle
        while (AnyBusy())
            yield return null;

        Tick();

        // wait until Enemies become idle
        while (AnyBusy())
            yield return null;

        IsProcessingTurn = false;
    }

    public void Register(CombatantAnimator animator) => m_Animators.Add(animator);

    public void Unregister(CombatantAnimator animator) => m_Animators.Remove(animator);

    public void Tick()
    {
        m_TurnCount++;
        OnTick?.Invoke();
    }

    public void BeginTurn()
    {
        if (IsProcessingTurn)
            return;

        StartCoroutine(ProcessTurnCoroutine());
    }
}
