using System;
using System.Collections.Generic;

namespace Loot
{
    public static class WeightedPool
    {
        public static T PickRandom<T>(IReadOnlyList<T> pool, Func<T, float> weightSelector)
        {
            float total = 0f;
            for (int i = 0; i < pool.Count; i++)
                total += weightSelector(pool[i]);

            float roll = UnityEngine.Random.value * total;
            float acc = 0f;

            for (int i = 0; i < pool.Count; i++)
            {
                acc += weightSelector(pool[i]);
                if (roll < acc)
                    return pool[i];
            }

            return pool[pool.Count - 1];
        }
    }
}
