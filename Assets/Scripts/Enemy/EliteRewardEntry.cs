using System;
using Combat;

namespace Enemy
{
    [Serializable]
    public class EliteRewardEntry
    {
        public CombatantStat Stat;
        public int Amount;
        public float Weight;
    }
}
