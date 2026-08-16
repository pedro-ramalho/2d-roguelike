using System;
using UnityEngine;

[Serializable]
public struct LevelEntry<T>
{
    public T Prefab;
    public int MinCount;
    public int MaxCount;
}
