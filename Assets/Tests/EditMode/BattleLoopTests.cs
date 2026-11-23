using System.Reflection;
using Game.Services.Config;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ScreenNavigation.Tests
{
    /// <summary>Battle → reward from the config → result over the dimmed battle → back to the menu.</summary>
    public sealed class BattleLoopTests
    {
        private GameObject root;
        private LevelView level;
        private Button winButton;
        private Button loseButton;
        private BattleResultPanel resultPanel;
        private TMP_Text titleLabel;
        private TMP_Text rewardLabel;
        private Button backToMenuButton;
        private RecordingNavigator navigator;
        private FakeWallet wallet;
        private FakeConfig config;

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("Battle loop tests");
            root.SetActive(false);

            level = Create<LevelView>(root.transform, "Level");
            winButton = Create<Button>(level.transform, "Win");
            loseButton = Create<Button>(level.transform, "Lose");
            resultPanel = Create<BattleResultPanel>(level.transform, "Result");
            titleLabel = Create<TextMeshProUGUI>(resultPanel.transform, "Title");
            rewardLabel = Create<TextMeshProUGUI>(resultPanel.transform, "Reward");
            backToMenuButton = Create<Button>(resultPanel.transform, "Back to menu");
            Assign(level, "backButton", Create<Button>(level.transform, "Back"));
            Assign(level, "winButton", winButton);
            Assign(level, "loseButton", loseButton);
            Assign(level, "resultPanel", resultPanel);
            Assign(resultPanel, "titleLabel", titleLabel);
            Assign(resultPanel, "rewardLabel", rewardLabel);
            Assign(resultPanel, "backToMenuButton", backToMenuButton);

            root.SetActive(true);
            // Edit Mode does not run Awake, so call it like Play Mode would.
            InvokeAwake(level);
            InvokeAwake(resultPanel);
            resultPanel.Hide(); // hidden until the battle ends, as in the scene

            navigator = new RecordingNavigator();
            wallet = new FakeWallet();
            config = new FakeConfig(win: 150, lose: 30);
        }

        [TearDown]
        public void TearDown()
        {
            config.Dispose();
            Object.DestroyImmediate(root);
        }

        [Test]
        public void AWinPaysTheWinRewardFromTheConfig()
        {
            var paid = new BattleRewardGranter(config, wallet).Grant(BattleOutcome.Win);

            Assert.AreEqual(150, paid);
            Assert.AreEqual(150, wallet.Cash);
        }

        [Test]
        public void ALossThatPaysNothingLeavesTheWalletAlone()
        {
            using var freeLoss = new FakeConfig(win: 150, lose: 0);

            var paid = new BattleRewardGranter(freeLoss, wallet).Grant(BattleOutcome.Lose);

            Assert.AreEqual(0, paid);
            Assert.AreEqual(0, wallet.AddCount);
        }

        [TestCase(BattleOutcome.Win, 150, "Victory!")]
        [TestCase(BattleOutcome.Lose, 30, "Defeat")]
        public void FinishingABattlePaysAndShowsTheResultOverTheBattle(BattleOutcome outcome, int reward, string title)
        {
            var controller = StartLevel();

            (outcome == BattleOutcome.Win ? winButton : loseButton).onClick.Invoke();

            Assert.AreEqual(reward, wallet.Cash);
            Assert.IsTrue(level.IsResultShown);
            Assert.AreEqual(title, titleLabel.text);
            Assert.AreEqual($"+{reward}", rewardLabel.text);
            Assert.IsNull(navigator.LastPushed, "the battle stays on screen under the result");
            controller.Dispose();
        }

        [Test]
        public void ABattlePaysOnlyOnce()
        {
            var controller = StartLevel();

            winButton.onClick.Invoke();
            winButton.onClick.Invoke();
            loseButton.onClick.Invoke();

            Assert.AreEqual(150, wallet.Cash);
            Assert.AreEqual(1, wallet.AddCount);
            controller.Dispose();
        }

        [Test]
        public void BackToMenuLeavesTheBattleAndTheNextBattleStartsClean()
        {
            var controller = StartLevel();
            winButton.onClick.Invoke();

            backToMenuButton.onClick.Invoke();
            Assert.AreEqual(typeof(MainMenuView), navigator.LastResetTo);

            level.Show(); // the next battle
            Assert.IsFalse(level.IsResultShown);
            loseButton.onClick.Invoke();
            Assert.AreEqual(180, wallet.Cash);
            controller.Dispose();
        }

        [Test]
        public void LevelStopsPayingAfterDispose()
        {
            var controller = StartLevel();
            controller.Dispose();

            winButton.onClick.Invoke();

            Assert.AreEqual(0, wallet.Cash);
            Assert.IsFalse(level.IsResultShown);
        }

        private LevelController StartLevel()
        {
            var controller = new LevelController(level, new LevelModel(), navigator, new BattleRewardGranter(config, wallet));
            controller.Start();
            return controller;
        }

        private static T Create<T>(Transform parent, string name) where T : Component
        {
            var gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent);
            return gameObject.AddComponent<T>();
        }

        private static void Assign(Object target, string field, Object value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(field).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void InvokeAwake(MonoBehaviour behaviour) =>
            behaviour.GetType().GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(behaviour, null);
    }
}
