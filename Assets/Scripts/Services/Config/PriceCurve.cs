using System;
using UnityEngine;

namespace Game.Services.Config
{
    /// <summary>
    /// A price that grows with every purchase: price = round(baseCost × growth^level),
    /// where level is how many were already bought. The same formula as the economy sheet.
    /// Reusable for any repeated purchase: Skill tree upgrades now, minion prices later.
    /// </summary>
    [Serializable]
    public sealed class PriceCurve
    {
        [Tooltip("Price of the first purchase (level 0).")]
        [SerializeField, Min(1)] private int baseCost;

        [Tooltip("How many times more each next purchase costs. 1 = flat price.")]
        [SerializeField, Min(1f)] private float growth;

        [Tooltip("How many times it can be bought.")]
        [SerializeField, Min(1)] private int maxLevel;

        /// <summary>Used by Unity when it creates the asset. The values come from the Inspector.</summary>
        public PriceCurve() { }

        public PriceCurve(int baseCost, float growth, int maxLevel)
        {
            this.baseCost = baseCost;
            this.growth = growth;
            this.maxLevel = maxLevel;
            Validate();
        }

        public int BaseCost => baseCost;
        public float Growth => growth;
        public int MaxLevel => maxLevel;

        public bool IsMaxed(int level) => level >= maxLevel;

        /// <summary>Price of the next purchase when <paramref name="level"/> were already bought.</summary>
        public int PriceAt(int level)
        {
            if (level < 0)
                throw new ArgumentOutOfRangeException(nameof(level), level, "Level can't be negative.");

            // double, so the sheet and the game round the same way (338, not 337).
            var price = baseCost * Math.Pow(growth, level);
            return price >= int.MaxValue ? int.MaxValue : (int)Math.Round(price, MidpointRounding.AwayFromZero);
        }

        internal void Validate()
        {
            if (baseCost < 1) throw new InvalidOperationException($"Price curve: base cost must be at least 1 (is {baseCost}).");
            if (growth < 1f) throw new InvalidOperationException($"Price curve: growth must be at least 1 (is {growth}).");
            if (maxLevel < 1) throw new InvalidOperationException($"Price curve: max level must be at least 1 (is {maxLevel}).");
        }
    }
}
