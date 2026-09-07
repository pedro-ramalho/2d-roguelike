using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Status;
using UnityEngine;

namespace Combat
{
    public class CombatantStatusEffects : MonoBehaviour
    {
        [SerializeField]
        private StatusEffectRoll[] m_StatusRolls;

        private readonly List<StatusEffect> m_StatusEffects = new();

        private Combatant m_Combatant;
        private TurnManager m_TurnManager;

        public IReadOnlyList<StatusEffect> StatusEffects => m_StatusEffects;

        public event Action<StatusEffect> Applied;
        public event Action<StatusEffect> Removed;
        public event Action Ticked;

        public bool IsStunned => m_StatusEffects.Any(e => e.Type == StatusEffectType.Stunned); // RUNTIME FLAG

        void Start()
        {
            m_Combatant = GetComponent<Combatant>();

            m_TurnManager = GameManager.Instance.TurnManager;

            m_TurnManager.OnTick += TickStatusEffects;
        }

        void OnDestroy()
        {
            if (m_TurnManager != null)
                m_TurnManager.OnTick -= TickStatusEffects;
        }

        public void Apply(StatusEffect effect)
        {
            foreach (StatusEffect sf in m_StatusEffects)
            {
                if (sf.Type == effect.Type)
                {
                    sf.Duration = Mathf.Max(sf.Duration, effect.Duration);
                    Applied?.Invoke(effect);

                    return;
                }
            }

            m_StatusEffects.Add(effect);
            effect.OnApplied(m_Combatant);

            Applied?.Invoke(effect);
        }

        public void Remove(StatusEffect effect)
        {
            m_StatusEffects.Remove(effect);
            effect.OnRemoved(m_Combatant);

            Removed?.Invoke(effect);
        }

        internal void RollStatusOnHit(Combatant target)
        {
            int level = GameManager.Instance.LevelManager.CurrentLevel;

            float maxChance = GameManager.Instance.ProgressionSettings.MaxStatusChance;

            foreach (StatusEffectRoll roll in m_StatusRolls)
            {
                float probability = Mathf.Clamp(
                    roll.BaseProbability + roll.PerLevelBonus * level,
                    0,
                    maxChance
                );

                if (UnityEngine.Random.value < probability)
                {
                    Apply(StatusEffectFactory.FromType(roll.Type, roll.Duration));

                    return;
                }
            }
        }

        void TickStatusEffects()
        {
            for (int i = m_StatusEffects.Count - 1; i >= 0; i--)
            {
                StatusEffect effect = m_StatusEffects[i];
                effect.OnTurnEnd(m_Combatant);

                if (effect.IsDepleted)
                {
                    m_StatusEffects.RemoveAt(i);
                    effect.OnRemoved(m_Combatant);
                    Removed?.Invoke(effect);
                }
            }

            Ticked?.Invoke();
        }
    }
}
