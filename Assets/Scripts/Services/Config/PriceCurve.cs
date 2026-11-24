using System;
using UnityEngine;

namespace Game.Services.Config
{
    /// <summary>
    /// A price that grows with every purchase: price = round(baseCost × growth^level),
    /// where level is how many were already bought. The same formula as the economy sheet.
    /// Plain data from the Inspector (a struct: never null, nothing to create).
    /// Reusable for any repeated purchase: Skill tree upgrades now, minion prices later.
    /// </summary>
    [Serializable]
    public struct PriceCurve
    {
        [Tooltip("Price of the first purchase (level 0).")]
        [SerializeField, Min(1)] private int baseCost;

        [Tooltip("How many times more each next purchase costs. 1 = flat price.")]
        [SerializeField, Min(1f)] private float growth;

        [Tooltip("How many times it can be bought.")]
        [SerializeField, Min(1)] private int maxLevel;

        public PriceCurve(int baseCost, float growth, int maxLevel)
        {
            this.baseCost = baseCost;
            this.growth = growth;
            this.maxLevel = maxLevel;
        }

        public int BaseCost => baseCost;
        public float Growth => growth;
        public int MaxLevel => maxLevel;

        /// <summary>Base cost, growth and max level are all at least 1.</summary>
        public bool IsValid => baseCost >= 1 && growth >= 1f && maxLevel >= 1;

        public bool IsMaxed(int level) => level >= maxLevel;

        /// <summary>Price of the next purchase. Negative input is treated as level zero rather than throwing during gameplay.</summary>
        public int PriceAt(int level)
        {
            if (level < 0) level = 0;

            // double, so the sheet and the game round the same way (338, not 337).
            var price = baseCost * Math.Pow(growth, level);
            return price >= int.MaxValue ? int.MaxValue : (int)Math.Round(price, MidpointRounding.AwayFromZero);
        }
    }
}
