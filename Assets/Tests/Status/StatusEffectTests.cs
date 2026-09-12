using NUnit.Framework;
using Status;

namespace StatusTests
{
    public class StatusEffectTests
    {
        [TestCase(StatusEffectType.Weak)]
        [TestCase(StatusEffectType.Stunned)]
        [TestCase(StatusEffectType.Vulnerable)]
        [TestCase(StatusEffectType.Empowered)]
        public void OnTurnEnd_DecreasesDuration(StatusEffectType type)
        {
            int duration = 3;

            StatusEffect effect = StatusEffectFactory.FromType(type, duration);

            effect.OnTurnEnd(null);

            Assert.AreEqual(duration - 1, effect.Duration);
        }

        [TestCase(3, false)]
        [TestCase(1, false)]
        [TestCase(0, true)]
        [TestCase(-1, true)]
        public void IsDepleted_ReturnsTrueWhenDurationIsZeroOrLess(int duration, bool expected)
        {
            StatusEffect effect = StatusEffectFactory.FromType(StatusEffectType.Weak, duration);

            Assert.AreEqual(expected, effect.IsDepleted);
        }

        [TestCase(StatusEffectType.Weak, true)]
        [TestCase(StatusEffectType.Stunned, true)]
        [TestCase(StatusEffectType.Vulnerable, true)]
        [TestCase(StatusEffectType.Empowered, false)]
        public void IsNegative_ReflectsStatusType(StatusEffectType type, bool expected)
        {
            StatusEffect effect = StatusEffectFactory.FromType(type, 1);

            Assert.AreEqual(expected, effect.IsNegative);
        }

        [TestCase(StatusEffectType.Weak, 10f, 10f)]
        [TestCase(StatusEffectType.Stunned, 10f, 10f)]
        [TestCase(StatusEffectType.Vulnerable, 10f, 15f)]
        [TestCase(StatusEffectType.Empowered, 10f, 10f)]
        public void ModifyIncomingDamage_ReturnsExpectedDamage(
            StatusEffectType type,
            float baseDamage,
            float expectedIncomingDamage
        )
        {
            StatusEffect effect = StatusEffectFactory.FromType(type, 1);

            Assert.AreEqual(expectedIncomingDamage, effect.ModifyIncomingDamage(baseDamage));
        }

        [TestCase(StatusEffectType.Weak, 10f, 5f)]
        [TestCase(StatusEffectType.Stunned, 10f, 10f)]
        [TestCase(StatusEffectType.Vulnerable, 10f, 10f)]
        [TestCase(StatusEffectType.Empowered, 10f, 15f)]
        public void ModifyOutgoingDamage_ReturnsExpectedDamage(
            StatusEffectType type,
            float baseDamage,
            float expectedOutgoingDamage
        )
        {
            StatusEffect effect = StatusEffectFactory.FromType(type, 1);

            Assert.AreEqual(expectedOutgoingDamage, effect.ModifyOutgoingDamage(baseDamage));
        }

        [TestCase(StatusEffectType.Weak, "WEAK")]
        [TestCase(StatusEffectType.Stunned, "STUN")]
        [TestCase(StatusEffectType.Vulnerable, "VULN")]
        [TestCase(StatusEffectType.Empowered, "EMPW")]
        public void ToString_ReturnsExpectedLabel(StatusEffectType type, string expected)
        {
            StatusEffect effect = StatusEffectFactory.FromType(type, 1);

            Assert.AreEqual(expected, effect.ToString());
        }
    }
}
