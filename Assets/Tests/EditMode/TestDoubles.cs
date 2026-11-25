using System;
using Game.Services.Config;
using Game.Services.Wallet;
using Object = UnityEngine.Object;

namespace Game.ScreenNavigation.Tests
{
    /// <summary>Records where a controller asked to navigate, instead of showing screens.</summary>
    internal sealed class RecordingNavigator : IScreenNavigator
    {
        public Type LastPushed { get; private set; }
        public Type LastReplaced { get; private set; }
        public Type LastResetTo { get; private set; }
        public int PushCount { get; private set; }
        public int PopCount { get; private set; }

        public bool Push<TView>() where TView : ScreenView
        {
            LastPushed = typeof(TView);
            PushCount++;
            return true;
        }

        public bool Pop()
        {
            PopCount++;
            return true;
        }

        public bool Replace<TView>() where TView : ScreenView
        {
            LastReplaced = typeof(TView);
            return true;
        }

        public bool ResetTo<TView>() where TView : ScreenView
        {
            LastResetTo = typeof(TView);
            return true;
        }
    }

    /// <summary>A wallet in memory, with the same rule as WalletService: only positive amounts.</summary>
    internal sealed class FakeWallet : IWalletService
    {
        public int Cash { get; private set; }
        public int AddCount { get; private set; }
        public event Action<int> CashChanged;

        public void Add(int amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), amount, "Cash amounts must be positive.");
            Cash += amount;
            AddCount++;
            CashChanged?.Invoke(Cash);
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), amount, "Cash amounts must be positive.");
            if (amount > Cash) return false;
            Cash -= amount;
            CashChanged?.Invoke(Cash);
            return true;
        }
    }

    /// <summary>A config built in code. Dispose it: domain reload is off, so the asset must be destroyed.</summary>
    internal sealed class FakeConfig : IConfigService, IDisposable
    {
        public FakeConfig(int win, int lose) =>
            Economy = EconomyConfig.Create(new BattleRewards(win, lose), new PriceCurve(baseCost: 100, growth: 1.5f, maxLevel: 10));

        public System.Collections.Generic.IReadOnlyList<Game.Services.Skills.SkillTreeDefinition> SkillTrees =>
            Array.Empty<Game.Services.Skills.SkillTreeDefinition>();

        private EconomyConfig Economy { get; }
        IEconomyConfig IConfigService.Economy => Economy;

        public void Dispose()
        {
            if (Economy != null) Object.DestroyImmediate(Economy);
        }
    }
}
