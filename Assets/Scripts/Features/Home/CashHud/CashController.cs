using System;
using Game.Services.Wallet;
using VContainer.Unity;

namespace Game.Hud
{
    /// <summary>Connects the Cash label to the wallet: the label follows every change.</summary>
    public sealed class CashController : IStartable, IDisposable
    {
        private readonly CashView view;
        private readonly IWalletService wallet;

        public CashController(CashView view, IWalletService wallet)
        {
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.wallet = wallet ?? throw new ArgumentNullException(nameof(wallet));
        }

        public void Start()
        {
            view.ShowCash(wallet.Cash);
            wallet.CashChanged += view.ShowCash;
        }

        // The wallet outlives this scene, so unsubscribe or it would call a destroyed view.
        public void Dispose()
        {
            wallet.CashChanged -= view.ShowCash;
        }
    }
}
