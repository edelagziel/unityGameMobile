using Game.Services.Config;
using NUnit.Framework;
using UnityEditor;

namespace Game.Services.Tests
{
    /// <summary>The one check of the real numbers: a wrong value in the asset fails here, before a build.</summary>
    public sealed class EconomyConfigAssetTests
    {
        private const string AssetPath = "Assets/Config/EconomyConfig.asset";

        [Test]
        public void TheShippedEconomyConfigIsValid()
        {
            var economy = AssetDatabase.LoadAssetAtPath<EconomyConfig>(AssetPath);

            Assert.IsNotNull(economy, $"{AssetPath} is missing.");
            Assert.IsNull(economy.FindProblem(), economy.FindProblem());
        }

        [Test]
        public void AnEmptyConfigReportsAProblemInsteadOfThrowing()
        {
            var economy = EconomyConfig.Create(default, default);
            try
            {
                Assert.IsNotNull(economy.FindProblem());
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(economy);
            }
        }
    }
}
