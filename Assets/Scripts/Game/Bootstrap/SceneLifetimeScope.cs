using UnityEngine;
using VContainer.Unity;

namespace Game.Bootstrap
{
    /// <summary>
    /// Base for every scene scope that needs services. Its parent is the Bootstrap scope,
    /// so its controllers get the services by constructor injection,
    /// however the scene was opened (by Bootstrap or by going back from Level).
    /// </summary>
    public abstract class SceneLifetimeScope : LifetimeScope
    {
        protected override LifetimeScope FindParent()
        {
            var bootstrap = Find<BootstrapLifetimeScope>();
            if (bootstrap == null)
                Debug.LogError($"{name}: the Bootstrap scope is missing. Start Play from the Bootstrap scene.", this);
            return bootstrap;
        }
    }
}
