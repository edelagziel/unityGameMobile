using UnityEngine;

namespace Game.Services.Config
{
    /// <summary>
    /// Every economy number of the game, in one asset (Assets/Config/EconomyConfig.asset).
    /// Designers change the numbers in the Inspector; no script holds an economy number.
    /// Read it through <see cref="IConfigService.Economy"/>, never by a direct reference to the asset.
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Economy Config", fileName = "EconomyConfig")]
    public sealed class EconomyConfig : ScriptableObject
    {
        [Header("Sources · Cash in")]
        [SerializeField] private BattleRewards battleRewards;

        [Header("Sinks · Cash out")]
        [Tooltip("The Skill tree upgrade: the big sink.")]
        [SerializeField] private PriceCurve upgrade;

        public BattleRewards BattleRewards => battleRewards;
        public PriceCurve Upgrade => upgrade;

        /// <summary>Price of the next upgrade when <paramref name="level"/> were already bought. Same name as in the Lesson 4 slides.</summary>
        public int UpgradeCost(int level) => upgrade.PriceAt(level);

        /// <summary>
        /// The one check of the numbers: what is wrong, or null if they are fine. It never throws.
        /// Bootstrap logs it as an error at startup; an EditMode test checks the shipped asset.
        /// </summary>
        public string FindProblem()
        {
            if (!battleRewards.IsValid)
                return $"Battle rewards: a win must pay at least 1 and a loss from 0 up to a win (win {battleRewards.Win}, loss {battleRewards.Lose}).";
            if (!upgrade.IsValid)
                return $"Upgrade: base cost, growth and max level must all be at least 1 (base {upgrade.BaseCost}, growth {upgrade.Growth}, max {upgrade.MaxLevel}).";
            return null;
        }

        /// <summary>
        /// A config built in code instead of loaded from the asset: for tests now,
        /// and later for a remote config that puts server values on top of the asset.
        /// </summary>
        public static EconomyConfig Create(BattleRewards battleRewards, PriceCurve upgrade)
        {
            var config = CreateInstance<EconomyConfig>();
            config.battleRewards = battleRewards;
            config.upgrade = upgrade;
            return config;
        }
    }
}
