using System;
using Game.Services.Skills;
using Game.Services.Wallet;
using VContainer.Unity;

namespace Game.ScreenNavigation
{
    public sealed class SkillTreeController : IStartable, IDisposable
    {
        private readonly ISkillTreeView view;
        private readonly ISkillTreeModel model;
        private readonly IScreenNavigator navigator;
        private readonly IWalletService wallet;
        private bool started;
        private bool purchasing;

        public SkillTreeController(ISkillTreeView view, ISkillTreeModel model, IScreenNavigator navigator, IWalletService wallet)
        {
            this.view = view;
            this.model = model;
            this.navigator = navigator;
            this.wallet = wallet;
        }

        public void Start()
        {
            if (started) return;
            started = true;
            view.TabPressed += ShowTab;
            view.Shown += OnShown;
            view.Selected += Select;
            view.CycleRequested += Cycle;
            view.PurchaseRequested += Purchase;
            wallet.CashChanged += BalanceChanged;
            if (view.Visible) OnShown();
        }

        public void Dispose()
        {
            if (!started) return;
            started = false;
            view.TabPressed -= ShowTab;
            view.Shown -= OnShown;
            view.Selected -= Select;
            view.CycleRequested -= Cycle;
            view.PurchaseRequested -= Purchase;
            wallet.CashChanged -= BalanceChanged;
        }

        public bool Open(SkillContext context)
        {
            if (!model.Open(context)) return false;
            return navigator.ResetTo<SkillTreeView>();
        }

        private void ShowTab() => navigator.ResetTo<SkillTreeView>();
        private void BalanceChanged(int cash) { if (view.Visible && !purchasing) Refresh(); }
        private void OnShown() { view.SelectTab(); Refresh(); }
        private void Select(string id)
        {
            model.Select(id);
            view.ShowSelection(model.Selection);
        }
        private void Cycle() { model.Cycle(); Refresh(); }
        private void Purchase(SkillContext context, string id, int target)
        {
            // The wallet publishes CashChanged synchronously during a purchase.
            purchasing = true;
            string feedback;
            try { feedback = model.Purchase(context, id, target); }
            finally { purchasing = false; }
            Refresh(feedback ?? "Upgrade purchased.");
        }

        private void Refresh(string feedback = null)
        {
            if (model.Tree != null)
                view.Render(model.Context, model.Tree, model.Selection, model.Offers(), wallet.Cash, feedback);
        }
    }
}
