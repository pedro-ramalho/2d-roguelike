using System;
using UnityEngine;

namespace Status
{
    [CreateAssetMenu(fileName = "StatusEffectIconSet", menuName = "Status/Icon Set")]
    public class StatusEffectIconSet : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public StatusEffectType Type;
            public Sprite Icon;
        }

        [SerializeField]
        private Entry[] m_Entries;

        public Sprite For(StatusEffectType type)
        {
            foreach (Entry e in m_Entries)
                if (e.Type == type)
                    return e.Icon;

            return null;
        }
    }
}
