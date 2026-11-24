using System;
using System.Collections.Generic;
using System.Threading;
using Game.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Game.Bootstrap
{
    /// <summary>
    /// Starts every service once, in the order they are registered in <see cref="BootstrapLifetimeScope"/>,
    /// then opens the first scene. Nothing loads the Bootstrap scene again, so this runs once per launch.
    /// </summary>
    public sealed class BootstrapFlow : IAsyncStartable
    {
        private readonly IReadOnlyList<IInitializableService> services;
        private readonly string firstScene;

        public BootstrapFlow(IReadOnlyList<IInitializableService> services, string firstScene)
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
            if (string.IsNullOrEmpty(firstScene))
                throw new ArgumentException("Set First Scene on BootstrapLifetimeScope.", nameof(firstScene));
            this.firstScene = firstScene;
        }

        public async Awaitable StartAsync(CancellationToken cancellation)
        {
            for (var i = 0; i < services.Count; i++)
            {
                await services[i].InitializeAsync(cancellation);
                Debug.Log($"Bootstrap: {i + 1} · {services[i].GetType().Name} ready");
            }

            Debug.Log($"Bootstrap: services ready, opening {firstScene}");
            await SceneManager.LoadSceneAsync(firstScene);
        }
    }
}
