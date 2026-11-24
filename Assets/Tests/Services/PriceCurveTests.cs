using Game.Services.Config;
using NUnit.Framework;

namespace Game.Services.Tests
{
    public sealed class PriceCurveTests
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
            Assert.AreEqual(expectedPrice, curve.PriceAt(alreadyBought));
        }

        [Test]
        public void GrowthOfOneKeepsTheSamePrice()
        {
            var curve = new PriceCurve(baseCost: 40, growth: 1f, maxLevel: 5);
            Assert.AreEqual(40, curve.PriceAt(0));
            Assert.AreEqual(40, curve.PriceAt(4));
        }

        [Test]
        public void AHugePriceStopsAtIntMaxInsteadOfOverflowing()
        {
            var curve = new PriceCurve(baseCost: 1000, growth: 10f, maxLevel: 100);
            Assert.AreEqual(int.MaxValue, curve.PriceAt(50));
        }

        [Test]
        public void IsMaxedFromTheMaxLevel()
        {
            var curve = new PriceCurve(baseCost: 100, growth: 1.5f, maxLevel: 3);
            Assert.IsFalse(curve.IsMaxed(2));
            Assert.IsTrue(curve.IsMaxed(3));
        }

        [Test]
        public void ANegativeLevelUsesFirstPurchasePrice()
        {
            var curve = new PriceCurve(baseCost: 100, growth: 1.5f, maxLevel: 10);
            Assert.AreEqual(100, curve.PriceAt(-1));
        }

        [Test]
        public void TheSheetCurveIsValid() => Assert.IsTrue(new PriceCurve(100, 1.5f, 10).IsValid);

        [TestCase(0, 1.5f, 10)]
        [TestCase(100, 0.9f, 10)]
        [TestCase(100, 1.5f, 0)]
        public void WrongNumbersAreNotValid(int baseCost, float growth, int maxLevel) =>
            Assert.IsFalse(new PriceCurve(baseCost, growth, maxLevel).IsValid);

        [Test]
        public void AnEmptyCurveIsNotValid() => Assert.IsFalse(default(PriceCurve).IsValid);
    }
}
