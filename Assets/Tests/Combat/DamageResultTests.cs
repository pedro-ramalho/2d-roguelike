using Combat;
using NUnit.Framework;

namespace CombatTests
{
    public class DamageResultTests
    {
        [TestCase(3, 5)]
        [TestCase(0, 0)]
        public void Constructor_SetsBothProperties(int blockLost, int hpLost)
        {
            DamageResult result = new DamageResult(blockLost, hpLost);

            Assert.AreEqual(blockLost, result.BlockLost);
            Assert.AreEqual(hpLost, result.HPLost);
        }
    }
}
