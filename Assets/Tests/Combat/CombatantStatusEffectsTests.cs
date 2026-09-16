using System;
using System.Collections;
using System.Reflection;
using Combat;
using NUnit.Framework;
using Status;
using UnityEngine;
using UnityEngine.TestTools;

namespace CombatTests
{
    public class CombatantStatusEffectsTests
    {
        private GameObject m_GameObject;
        private CombatantStatusEffects m_Statuses;

        [SetUp]
        public void SetUp()
        {
            m_GameObject = new GameObject("StatusEffectsTest");
            m_Statuses = m_GameObject.AddComponent<CombatantStatusEffects>();
        }

        [TearDown]
        public void TearDown() => UnityEngine.Object.DestroyImmediate(m_GameObject);

        private static void InvokePrivate(object target, string methodName)
        {
            MethodInfo method = target
                .GetType()
                .GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

            method.Invoke(target, null);
        }

        [Test]
        public void Apply_NewEffect_AddsToList()
        {
            StatusEffect effect = new StatusEffectWeak(3);

            m_Statuses.Apply(effect);

            Assert.AreEqual(1, m_Statuses.StatusEffects.Count);
            Assert.AreSame(effect, m_Statuses.StatusEffects[0]);
        }

        [Test]
        public void Apply_SameTypeWithHigherDuration_UpdatesDuration()
        {
            StatusEffect effect = new StatusEffectWeak(2);

            m_Statuses.Apply(effect);
            m_Statuses.Apply(new StatusEffectWeak(5));

            Assert.AreEqual(1, m_Statuses.StatusEffects.Count);
            Assert.AreEqual(5, m_Statuses.StatusEffects[0].Duration);
        }

        [Test]
        public void Apply_SameTypeLowerDuration_DoesNotUpdateDuration()
        {
            StatusEffect effect = new StatusEffectWeak(5);

            m_Statuses.Apply(effect);
            m_Statuses.Apply(new StatusEffectWeak(2));

            Assert.AreEqual(1, m_Statuses.StatusEffects.Count);
            Assert.AreEqual(5, m_Statuses.StatusEffects[0].Duration);
        }

        [Test]
        public void Apply_DifferentTypes_BothAddedToList()
        {
            m_Statuses.Apply(new StatusEffectWeak(2));
            m_Statuses.Apply(new StatusEffectEmpowered(2));

            Assert.AreEqual(2, m_Statuses.StatusEffects.Count);
        }

        [Test]
        public void Apply_FiresAppliedEvent()
        {
            StatusEffect captured = null;

            m_Statuses.Applied += arg => captured = arg;

            StatusEffect effect = new StatusEffectWeak(1);

            m_Statuses.Apply(effect);

            Assert.AreSame(effect, captured);
        }

        [Test]
        public void Remove_RemovesFromList()
        {
            StatusEffect effect = new StatusEffectWeak(3);

            m_Statuses.Apply(effect);
            m_Statuses.Remove(effect);

            Assert.AreEqual(0, m_Statuses.StatusEffects.Count);
        }

        [Test]
        public void Remove_FiresRemovedEvent()
        {
            StatusEffect effect = new StatusEffectWeak(3);

            m_Statuses.Apply(effect);

            StatusEffect captured = null;

            m_Statuses.Removed += arg => captured = arg;

            m_Statuses.Remove(effect);

            Assert.AreSame(effect, captured);
        }

        [Test]
        public void IsStunned_NoEffects_ReturnsFalse() => Assert.IsFalse(m_Statuses.IsStunned);

        [Test]
        public void IsStunned_WithStunnedApplied_ReturnsTrue()
        {
            m_Statuses.Apply(new StatusEffectStunned(2));

            Assert.IsTrue(m_Statuses.IsStunned);
        }

        [Test]
        public void IsStunned_WithoutStunnedApplied_ReturnsFalse()
        {
            m_Statuses.Apply(new StatusEffectWeak(2));

            Assert.IsFalse(m_Statuses.IsStunned);
        }

        [Test]
        public void TickStatusEffects_DecrementsDurations()
        {
            m_Statuses.Apply(new StatusEffectWeak(3));

            InvokePrivate(m_Statuses, "TickStatusEffects");

            Assert.AreEqual(2, m_Statuses.StatusEffects[0].Duration);
        }

        [Test]
        public void TickStatusEffects_RemovesDepletedEffects()
        {
            m_Statuses.Apply(new StatusEffectWeak(1));

            InvokePrivate(m_Statuses, "TickStatusEffects");

            Assert.AreEqual(0, m_Statuses.StatusEffects.Count);
        }

        [Test]
        public void TickStatusEffects_DepletedEffect_FiresRemovedEvent()
        {
            StatusEffect effect = new StatusEffectWeak(1);

            m_Statuses.Apply(effect);

            StatusEffect captured = null;

            m_Statuses.Removed += arg => captured = arg;

            InvokePrivate(m_Statuses, "TickStatusEffects");

            Assert.AreSame(effect, captured);
        }

        [Test]
        public void TickStatusEffects_KeepsActive_RemovesDepleted()
        {
            m_Statuses.Apply(new StatusEffectWeak(1));
            m_Statuses.Apply(new StatusEffectEmpowered(3));

            InvokePrivate(m_Statuses, "TickStatusEffects");

            Assert.AreEqual(1, m_Statuses.StatusEffects.Count);
            Assert.AreEqual(StatusEffectType.Empowered, m_Statuses.StatusEffects[0].Type);
        }

        [Test]
        public void TickStatusEffects_FiresTickedEvent()
        {
            bool fired = false;

            m_Statuses.Ticked += () => fired = true;

            InvokePrivate(m_Statuses, "TickStatusEffects");

            Assert.IsTrue(fired);
        }
    }
}
