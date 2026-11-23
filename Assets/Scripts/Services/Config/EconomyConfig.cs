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
        [SerializeField] private BattleRewards battleRewards = new BattleRewards();

        [Header("Sinks · Cash out")]
        [Tooltip("The Skill tree upgrade: the big sink.")]
        [SerializeField] private PriceCurve upgrade = new PriceCurve();

        public BattleRewards BattleRewards => battleRewards;
        public PriceCurve Upgrade => upgrade;

        /// <summary>Price of the next upgrade when <paramref name="level"/> were already bought. Same name as in the Lesson 4 slides.</summary>
        public int UpgradeCost(int level) => upgrade.PriceAt(level);

        /// <summary>
        /// A config built in code instead of loaded from the asset: for tests now,
        /// and later for a remote config that puts server values on top of the asset.
        /// </summary>
        public static EconomyConfig Create(BattleRewards battleRewards, PriceCurve upgrade)
        {
            var config = CreateInstance<EconomyConfig>();
            config.battleRewards = battleRewards ?? throw new System.ArgumentNullException(nameof(battleRewards));
            config.upgrade = upgrade ?? throw new System.ArgumentNullException(nameof(upgrade));
            return config;
        }

        /// <summary>Throws with a clear message if a number in the asset is missing or wrong.</summary>
        internal void Validate()
        {
            battleRewards.Validate();
            upgrade.Validate();
        }
    }
}
