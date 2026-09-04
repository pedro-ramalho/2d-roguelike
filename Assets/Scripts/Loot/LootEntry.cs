using System;
using Food;

namespace Loot
{
    public enum LootKind
    {
        Nothing,
        Food,
        Stamina,
    }

    [Serializable]
    public class LootEntry
    {
        public LootKind Kind;
        public int Weight;
        public FoodObject FoodPrefab;
        public int StaminaAmount;
    }
}
