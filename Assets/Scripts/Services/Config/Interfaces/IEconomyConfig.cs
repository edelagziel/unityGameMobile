namespace Game.Services.Config
{
    public interface IEconomyConfig
    {
        BattleRewards BattleRewards { get; }
        PriceCurve Upgrade { get; }
        int UpgradeCost(int level);
    }
}
