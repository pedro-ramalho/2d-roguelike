using System.Reflection;
using Combat;
using NUnit.Framework;
using UnityEngine;

namespace CombatTests
{
    public class CombatantStatsTests
    {
        private GameObject m_GameObject;
        private CombatantStats m_Stats;

        private static void SetPrivate(object target, string fieldName, object value)
        {
            FieldInfo field = target
                .GetType()
                .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(target, value);
        }

        private static void SetInitial(
            CombatantStats stats,
            int maxHP,
            int maxBlock,
            int attack,
            int maxStamina
        )
        {
            SetPrivate(stats, "m_InitialMaxHP", maxHP);
            SetPrivate(stats, "m_InitialMaxBlock", maxBlock);
            SetPrivate(stats, "m_InitialAttack", attack);
            SetPrivate(stats, "m_InitialMaxStamina", maxStamina);
        }

        [SetUp]
        public void SetUp()
        {
            m_GameObject = new GameObject("StatsTest");
            m_Stats = m_GameObject.AddComponent<CombatantStats>();

            SetInitial(m_Stats, maxHP: 20, maxBlock: 10, attack: 5, maxStamina: 3);

            m_Stats.Reset();
        }

        [TestCase(10, 3, 3, 0, 7, 20)]
        [TestCase(3, 10, 3, 7, 0, 13)]
        [TestCase(0, 5, 0, 5, 0, 15)]
        public void TakeDamage_ReturnsCorrectDamageResultAndUpdatesStats(
            int initialBlock,
            int damage,
            int expectedBlockLost,
            int expectedHpLost,
            int expectedBlockLeft,
            int expectedHpLeft
        )
        {
            m_Stats.AddBlock(initialBlock);

            DamageResult result = m_Stats.TakeDamage(damage);

            Assert.That(result.BlockLost, Is.EqualTo(expectedBlockLost));
            Assert.That(result.HPLost, Is.EqualTo(expectedHpLost));

            Assert.That(m_Stats.Block, Is.EqualTo(expectedBlockLeft));
            Assert.That(m_Stats.HP, Is.EqualTo(expectedHpLeft));
        }

        [Test]
        public void TakeDamage_FiresDamagedEvent()
        {
            bool fired = false;

            DamageResult result = default;

            m_Stats.Damaged += arg =>
            {
                fired = true;
                result = arg;
            };

            m_Stats.TakeDamage(3);

            Assert.IsTrue(fired);
            Assert.AreEqual(3, result.HPLost);
        }

        [TestCase(15, 10, 15)]
        [TestCase(5, 50, 20)]
        public void Heal_IncreasesHpWithoutExceedingMaxHp(int damage, int healing, int expectedHp)
        {
            m_Stats.TakeDamage(damage);

            m_Stats.Heal(healing);

            Assert.That(m_Stats.HP, Is.EqualTo(expectedHp));
        }

        [Test]
        public void Heal_FiresHealthAddedEvent()
        {
            int amount = -1;

            m_Stats.HealthAdded += arg => amount = arg;

            m_Stats.Heal(7);

            Assert.AreEqual(7, amount);
        }

        [TestCase(5, 5)]
        [TestCase(50, 10)]
        public void AddBlock_IncreasesBlockWithoutExceedingMaxBlock(int amount, int expectedBlock)
        {
            m_Stats.AddBlock(amount);

            Assert.That(m_Stats.Block, Is.EqualTo(expectedBlock));
        }

        [Test]
        public void AddBlock_FiresBlockAddedEvent()
        {
            int amount = -1;

            m_Stats.BlockAdded += arg => amount = arg;

            m_Stats.AddBlock(4);

            Assert.AreEqual(4, amount);
        }

        [TestCase(-1, 2)]
        [TestCase(-100, 0)]
        [TestCase(100, 3)]
        public void ChangeStamina_ClampsBetweenZeroAndMaxStamina(int amount, int expectedStamina)
        {
            m_Stats.ChangeStamina(amount);

            Assert.That(m_Stats.Stamina, Is.EqualTo(expectedStamina));
        }

        [Test]
        public void ChangeStamina_WhenStaminaReachesZero_FiresDepletedEvent()
        {
            bool depleted = false;

            m_Stats.Depleted += () => depleted = true;

            m_Stats.ChangeStamina(-3);

            Assert.IsTrue(depleted);
        }

        [Test]
        public void ChangeStamina_WhenStaminaIsAlreadyZero_DoesNotFireDepletedEvent()
        {
            m_Stats.ChangeStamina(-3);

            bool depleted = false;

            m_Stats.Depleted += () => depleted = true;

            m_Stats.ChangeStamina(0);

            Assert.IsFalse(depleted);
        }

        [TestCase(CombatantStat.MaxHP, 5, 25, 10, 5, 3)]
        [TestCase(CombatantStat.MaxBlock, 5, 20, 15, 5, 3)]
        [TestCase(CombatantStat.Attack, 5, 20, 10, 10, 3)]
        [TestCase(CombatantStat.MaxStamina, 5, 20, 10, 5, 8)]
        public void UpgradeStat_IncreasesSelectedStatOnly(
            CombatantStat stat,
            int amount,
            int expectedMaxHP,
            int expectedMaxBlock,
            int expectedAttack,
            int expectedMaxStamina
        )
        {
            m_Stats.UpgradeStat(stat, amount);

            Assert.AreEqual(expectedMaxHP, m_Stats.MaxHP);
            Assert.AreEqual(expectedMaxBlock, m_Stats.MaxBlock);
            Assert.AreEqual(expectedAttack, m_Stats.Attack);
            Assert.AreEqual(expectedMaxStamina, m_Stats.MaxStamina);
        }

        [Test]
        public void ApplyStatMultiplier_ScalesStats()
        {
            m_Stats.AddBlock(5);
            m_Stats.TakeDamage(10);

            m_Stats.ApplyStatMultiplier(hpMult: 1.5f, blockMult: 2f, attackMult: 1.5f);

            Assert.AreEqual(30, m_Stats.MaxHP);
            Assert.AreEqual(30, m_Stats.HP);
            Assert.AreEqual(20, m_Stats.MaxBlock);
            Assert.AreEqual(0, m_Stats.Block);
            Assert.AreEqual(8, m_Stats.Attack);
        }

        [Test]
        public void RefreshStats_RefillsHpAndStamina()
        {
            m_Stats.TakeDamage(15);
            m_Stats.ChangeStamina(-2);

            m_Stats.RefreshStats();

            Assert.AreEqual(20, m_Stats.HP);
            Assert.AreEqual(3, m_Stats.Stamina);
        }

        [Test]
        public void Reset_RestoresAllInitialValues()
        {
            m_Stats.TakeDamage(10);
            m_Stats.AddBlock(5);
            m_Stats.ChangeStamina(-2);
            m_Stats.UpgradeStat(CombatantStat.Attack, 100);

            m_Stats.Reset();

            Assert.AreEqual(20, m_Stats.MaxHP);
            Assert.AreEqual(20, m_Stats.HP);
            Assert.AreEqual(10, m_Stats.MaxBlock);
            Assert.AreEqual(0, m_Stats.Block);
            Assert.AreEqual(5, m_Stats.Attack);
            Assert.AreEqual(3, m_Stats.MaxStamina);
            Assert.AreEqual(3, m_Stats.Stamina);
        }

        [Test]
        public void Reset_FiresStatsResetEvent()
        {
            bool reset = false;

            m_Stats.StatsReset += () => reset = true;

            m_Stats.Reset();

            Assert.IsTrue(reset);
        }
    }
}
