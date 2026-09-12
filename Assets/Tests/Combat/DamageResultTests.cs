using System.Collections;
using Combat;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace CombatTests
{
    public class DamageResultTests
    {
        [Test]
        public void Constructor_SetsBothProperties()
        {
            DamageResult result = new DamageResult(3, 5);

            Assert.AreEqual(3, result.BlockLost);
            Assert.AreEqual(5, result.HPLost);
        }

        [Test]
        public void Construtor_AllowsZeroValues()
        {
            DamageResult result = new DamageResult(0, 0);

            Assert.AreEqual(0, result.BlockLost);
            Assert.AreEqual(0, result.HPLost);
        }
    }
}
