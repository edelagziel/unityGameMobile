using Game.Services.Config;
using NUnit.Framework;

namespace Game.ScreenNavigation.Tests
{
    public sealed class SkillTreeModelTests
    {
        // The Upgrade curve rows of the Lesson 4 economy sheet: base 100, growth 1.5.
        [TestCase(0, 100)]
        [TestCase(1, 150)]
        [TestCase(2, 225)]
        [TestCase(3, 338)]
        [TestCase(4, 506)]
        [TestCase(5, 759)]
        [TestCase(6, 1139)]
        [TestCase(7, 1709)]
        [TestCase(8, 2563)]
        [TestCase(9, 3844)]
        public void PricesMatchTheEconomySheet(int alreadyBought, int expectedPrice)
        {
            var curve = new PriceCurve(baseCost: 100, growth: 1.5f, maxLevel: 10);
            Assert.AreEqual(expectedPrice, SkillTreeModel.UpgradeCost(curve, alreadyBought));
        }

        [Test]
        public void GrowthOfOneKeepsTheSamePrice()
        {
            var curve = new PriceCurve(baseCost: 40, growth: 1f, maxLevel: 5);
            Assert.AreEqual(40, SkillTreeModel.UpgradeCost(curve, 0));
            Assert.AreEqual(40, SkillTreeModel.UpgradeCost(curve, 4));
        }

        [Test]
        public void AHugePriceStopsAtIntMaxInsteadOfOverflowing()
        {
            var curve = new PriceCurve(baseCost: 1000, growth: 10f, maxLevel: 100);
            Assert.AreEqual(int.MaxValue, SkillTreeModel.UpgradeCost(curve, 50));
        }

        [Test]
        public void IsMaxedFromTheMaxLevel()
        {
            var curve = new PriceCurve(baseCost: 100, growth: 1.5f, maxLevel: 3);
            Assert.IsFalse(SkillTreeModel.IsMaxed(curve, 2));
            Assert.IsTrue(SkillTreeModel.IsMaxed(curve, 3));
        }

        [Test]
        public void ANegativeLevelUsesFirstPurchasePrice()
        {
            var curve = new PriceCurve(baseCost: 100, growth: 1.5f, maxLevel: 10);
            Assert.AreEqual(100, SkillTreeModel.UpgradeCost(curve, -1));
        }
    }
}
