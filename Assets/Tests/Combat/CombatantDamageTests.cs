using System.Collections.Generic;
using Combat;
using NUnit.Framework;
using Status;

namespace CombatTests
{
    public class CombatantDamageTests
    {
        private static readonly IReadOnlyList<StatusEffect> NoStatuses = new StatusEffect[0];

        private static IReadOnlyList<StatusEffect> With(params StatusEffectType[] types)
        {
            StatusEffect[] effects = new StatusEffect[types.Length];

            for (int i = 0; i < types.Length; i++)
                effects[i] = StatusEffectFactory.FromType(types[i], 1);

            return effects;
        }

        static IEnumerable<TestCaseData> DamageCases()
        {
            yield return new TestCaseData(5, NoStatuses, NoStatuses, 5).SetName(
                "Damage is not modified when no statuses"
            );

            yield return new TestCaseData(0, NoStatuses, NoStatuses, 0).SetName(
                "Zero damage with no statuses returns 0"
            );

            yield return new TestCaseData(
                10,
                With(StatusEffectType.Empowered),
                NoStatuses,
                15
            ).SetName("Empowered attacker multiplies outgoing damage");

            yield return new TestCaseData(10, With(StatusEffectType.Weak), NoStatuses, 5).SetName(
                "Weak attacker halves outgoing damage"
            );

            yield return new TestCaseData(
                10,
                NoStatuses,
                With(StatusEffectType.Vulnerable),
                15
            ).SetName("Vulnerable defender multiplies incoming damage");

            yield return new TestCaseData(
                10,
                With(StatusEffectType.Empowered),
                With(StatusEffectType.Vulnerable),
                22
            ).SetName("Empowered and Vulnerable stack");

            yield return new TestCaseData(
                10,
                With(StatusEffectType.Weak),
                With(StatusEffectType.Vulnerable),
                7
            ).SetName("Weak and Vulnerable stack");

            yield return new TestCaseData(
                10,
                With(StatusEffectType.Stunned),
                NoStatuses,
                10
            ).SetName("Stunned does not affect damage");

            yield return new TestCaseData(3, With(StatusEffectType.Weak), NoStatuses, 1).SetName(
                "Fractional result floors down"
            );

            yield return new TestCaseData(1, With(StatusEffectType.Weak), NoStatuses, 0).SetName(
                "Less than 1 damage floors to zero (1 x0.5 -> 0)"
            );
        }

        [TestCaseSource(nameof(DamageCases))]
        public void ComputeDamage_ReturnsExpectedResult(
            int baseAttack,
            IReadOnlyList<StatusEffect> attackerStatuses,
            IReadOnlyList<StatusEffect> defenderStatuses,
            int expected
        )
        {
            int result = CombatantDamage.ComputeDamage(
                baseAttack,
                attackerStatuses,
                defenderStatuses
            );

            Assert.AreEqual(expected, result);
        }
    }
}
