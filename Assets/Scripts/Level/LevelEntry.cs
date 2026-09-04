using System;

namespace Level
{
    [Serializable]
    public struct LevelEntry<T>
    {
        public T Prefab;
        public int MinCount;
        public int MaxCount;
    }
}
