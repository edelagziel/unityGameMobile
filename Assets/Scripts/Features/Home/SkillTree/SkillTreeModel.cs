using System;
using Game.Services.Config;

namespace Game.ScreenNavigation
{
    public sealed class SkillTreeModel
    {
        public static bool IsMaxed(PriceCurve upgrade, int level) => level >= upgrade.MaxLevel;

        public static int UpgradeCost(PriceCurve upgrade, int level)
        {
            var price = upgrade.BaseCost * Math.Pow(upgrade.Growth, Math.Max(0, level));
            return price >= int.MaxValue ? int.MaxValue : (int)Math.Round(price, MidpointRounding.AwayFromZero);
        }
    }
}
