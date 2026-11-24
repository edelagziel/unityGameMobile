using System;
using Game.Bootstrap;
using Game.Hud;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game
{
    /// <summary>
    /// Scope for Home-specific dependencies. As a <see cref="SceneLifetimeScope"/>, its parent is the Bootstrap scope,
    /// so <see cref="CashController"/> receives the wallet service.
    /// </summary>
    public sealed class HomeLifetimeScope : SceneLifetimeScope
    {
        [SerializeField] private CashView cashView;

        protected override void Configure(IContainerBuilder builder)
        {
            if (cashView == null)
                throw new InvalidOperationException("Assign the CashView in HomeLifetimeScope.");

            builder.RegisterComponent(cashView);
            builder.RegisterEntryPoint<CashController>(Lifetime.Scoped);
        }
    }
}
