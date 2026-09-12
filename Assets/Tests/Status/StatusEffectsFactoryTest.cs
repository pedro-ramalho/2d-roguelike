using NUnit.Framework;
using Status;

namespace StatusTests
{
    public class StatusEffectsFactoryTest
    {
        [TestCase(StatusEffectType.Stunned, typeof(StatusEffectStunned), 1)]
        [TestCase(StatusEffectType.Weak, typeof(StatusEffectWeak), 2)]
        [TestCase(StatusEffectType.Vulnerable, typeof(StatusEffectVulnerable), 3)]
        [TestCase(StatusEffectType.Empowered, typeof(StatusEffectEmpowered), 4)]
        public void FromType_ValidType_ReturnsExpectedInstance(
            StatusEffectType type,
            System.Type expectedType,
            int duration
        )
        {
            StatusEffect effect = StatusEffectFactory.FromType(type, duration);

            Assert.IsInstanceOf(expectedType, effect);

            Assert.AreEqual(type, effect.Type);
            Assert.AreEqual(duration, effect.Duration);
        }

        [Test]
        public void FromType_InvalidType_ThrowsArgumentException() =>
            Assert.Throws<System.ArgumentException>(() =>
                StatusEffectFactory.FromType((StatusEffectType)999, 1)
            );
    }
}
