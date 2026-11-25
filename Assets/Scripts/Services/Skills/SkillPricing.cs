using System;
using Game.Services.Config;

namespace Game.Services.Skills
{
    public static class SkillPricing
    {
        public static int CurveCost(PriceCurve curve, int purchased)
        {
            double price = curve.BaseCost * Math.Pow(curve.Growth, Math.Max(0, purchased));
            return price >= int.MaxValue ? int.MaxValue : (int)Math.Round(price, MidpointRounding.AwayFromZero);
        }

        public static int Price(SkillLevelDefinition level, PriceCurve curve, int purchased) =>
            level.UseEconomyCurve ? CurveCost(curve, purchased) : level.FixedCash;
    }
}
