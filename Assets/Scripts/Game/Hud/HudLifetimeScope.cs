using System;
using Game.Bootstrap;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Hud
{
    /// <summary>
    /// Scope of the Home HUD. As a <see cref="SceneLifetimeScope"/>, its parent is the Bootstrap scope,
    /// so <see cref="CashController"/> receives the wallet service.
    /// </summary>
    public sealed class HudLifetimeScope : SceneLifetimeScope
    {
        [SerializeField] private CashView cashView;

        protected override void Configure(IContainerBuilder builder)
        {
            if (cashView == null)
                throw new InvalidOperationException("Assign the CashView in HudLifetimeScope.");

            builder.RegisterComponent(cashView);
            builder.RegisterEntryPoint<CashController>(Lifetime.Scoped);
        }
    }
}
