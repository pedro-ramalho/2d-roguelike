using System;
using UnityEngine;

namespace Combat
{
    public enum CombatantStat
    {
        MaxHP,
        MaxBlock,
        Attack,
        MaxStamina,
    }

    public class CombatantStats : MonoBehaviour
    {
        [Header("Initial Stats")]
        // Authored initial values
        [SerializeField]
        private int m_InitialMaxHP;

        [SerializeField]
        private int m_InitialMaxBlock;

        [SerializeField]
        private int m_InitialAttack;

        [SerializeField]
        private int m_InitialMaxStamina;

        [Header("Band Stats")]
        [SerializeField]
        private Level.BandStats[] m_BandStats;

        public int MaxHP { get; private set; }
        public int HP { get; private set; }
        public int MaxBlock { get; private set; }
        public int Block { get; private set; }
        public int Attack { get; private set; }
        public int MaxStamina { get; private set; }
        public int Stamina { get; private set; }

        public event Action<DamageResult> Damaged;
        public event Action<int> HealthAdded;
        public event Action<int> BlockAdded;
        public event Action<int> StaminaChanged;
        public event Action Depleted;
        public event Action StatsReset;

        public bool IsGodMode { get; set; }

        void Awake() => Reset();

        public DamageResult TakeDamage(int amount)
        {
            if (IsGodMode)
                return new DamageResult(0, 0);

            int blockLost = Mathf.Min(Block, amount);
            Block -= blockLost;

            int hpLost = Mathf.Max(0, amount - blockLost);
            HP -= hpLost;

            DamageResult result = new DamageResult(blockLost, hpLost);

            Damaged?.Invoke(result);

            return result;
        }

        public void Heal(int amount)
        {
            HP = Mathf.Clamp(HP + amount, 0, MaxHP);
            HealthAdded?.Invoke(amount);
        }

        public void AddBlock(int amount)
        {
            Block = Mathf.Clamp(Block + amount, 0, MaxBlock);
            BlockAdded?.Invoke(amount);
        }

        public void ChangeStamina(int amount)
        {
            int previous = Stamina;

            Stamina = Mathf.Clamp(Stamina + amount, 0, MaxStamina);
            StaminaChanged?.Invoke(Stamina);

            if (previous > 0 && Stamina == 0)
                Depleted?.Invoke();
        }

        public void UpgradeStat(CombatantStat stat, int amount)
        {
            switch (stat)
            {
                case CombatantStat.MaxHP:
                    MaxHP += amount;
                    break;
                case CombatantStat.MaxBlock:
                    MaxBlock += amount;
                    break;
                case CombatantStat.Attack:
                    Attack += amount;
                    break;
                case CombatantStat.MaxStamina:
                    MaxStamina += amount;
                    break;
            }
        }

        public void ApplyStatMultiplier(float hpMult, float blockMult, float attackMult)
        {
            MaxHP = Mathf.RoundToInt(MaxHP * hpMult);
            MaxBlock = Mathf.RoundToInt(MaxBlock * blockMult);
            Attack = Mathf.RoundToInt(Attack * attackMult);
            HP = MaxHP;
            Block = 0;
        }

        public void ApplyBandStats()
        {
            Level.BandType current = Core.GameManager.Instance.LevelManager.CurrentBand.Type;

            foreach (Level.BandStats entry in m_BandStats)
            {
                if (entry.Band == current)
                {
                    MaxHP = entry.MaxHP;
                    HP = entry.MaxHP;
                    Attack = entry.Attack;
                    MaxBlock = entry.MaxBlock;

                    return;
                }
            }
        }

        public void RefreshStats()
        {
            HP = MaxHP;
            Stamina = MaxStamina;
        }

        public void Reset()
        {
            MaxHP = m_InitialMaxHP;
            HP = m_InitialMaxHP;
            MaxBlock = m_InitialMaxBlock;
            Block = 0;
            Attack = m_InitialAttack;
            MaxStamina = m_InitialMaxStamina;
            Stamina = m_InitialMaxStamina;

            StatsReset?.Invoke();
        }
    }
}
