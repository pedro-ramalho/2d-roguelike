using System;
using UnityEngine;

public enum PlayerStat { MaxHP, MaxBlock, Attack, MaxStamina }

public class PlayerStats : MonoBehaviour
{
    // Player-specific stats
    [SerializeField] private int m_Stamina = 100;
    [SerializeField] private int m_MaxStamina = 100;

    // Properties
    public int Stamina => m_Stamina;
    public int MaxStamina => m_MaxStamina;

    // Events
    public event Action Depleted;
    public event Action<int> StaminaChanged;

    public void ChangeStamina(int amount)
    {
        int previous = m_Stamina;

        m_Stamina = Mathf.Clamp(m_Stamina + amount, 0, m_MaxStamina);

        StaminaChanged?.Invoke(m_Stamina);

        if (previous > 0 && m_Stamina == 0)
            Depleted?.Invoke();
    }

    public void DecrementStamina() => ChangeStamina(-1);

    public void ResetState() => m_Stamina = m_MaxStamina;

    public void IncreaseMaxStamina(int amount)
    {
        m_MaxStamina += amount;
        ResetState();
    }
}
