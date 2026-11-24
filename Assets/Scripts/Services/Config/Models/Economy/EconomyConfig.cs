using UnityEngine;

namespace Game.Services.Config
{
    /// <summary>
    /// Every economy number of the game, in one asset (Assets/Config/EconomyConfig.asset).
    /// Designers change the numbers in the Inspector; no script holds an economy number.
    /// Read it through <see cref="IConfigService.Economy"/>, never by a direct reference to the asset.
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Economy Config", fileName = "EconomyConfig")]
    public sealed class EconomyConfig : ScriptableObject, IEconomyConfig
    {
        [Header("Sources · Cash in")]
        [SerializeField] private BattleRewards battleRewards;

        [Header("Sinks · Cash out")]
        [Tooltip("The Skill tree upgrade: the big sink.")]
        [SerializeField] private PriceCurve upgrade;

        public BattleRewards BattleRewards => battleRewards;
        public PriceCurve Upgrade => upgrade;

        /// <summary>Constructs an in-memory asset with the supplied data.</summary>
        public static EconomyConfig Create(BattleRewards battleRewards, PriceCurve upgrade)
        {
            var config = CreateInstance<EconomyConfig>();
            config.battleRewards = battleRewards;
            config.upgrade = upgrade;
            return config;
        }
    }
}
