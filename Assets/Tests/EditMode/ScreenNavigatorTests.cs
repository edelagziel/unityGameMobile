using System;
using NUnit.Framework;
using UnityEngine;

namespace Game.ScreenNavigation.Tests
{
    public sealed class ScreenNavigatorTests
    {
        private GameObject root;
        private MainMenuView mainMenu;
        private SettingsView settings;
        private LevelView level;
        private ScreenNavigationHistory history;
        private ScreenNavigator navigator;

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("Navigation test screens");
            root.SetActive(false);
            var mainMenuObject = new GameObject("Main menu");
            var settingsObject = new GameObject("Settings");
            var levelObject = new GameObject("Level");
            mainMenuObject.SetActive(false);
            settingsObject.SetActive(false);
            levelObject.SetActive(false);
            mainMenu = mainMenuObject.AddComponent<MainMenuView>();
            settings = settingsObject.AddComponent<SettingsView>();
            level = levelObject.AddComponent<LevelView>();
            mainMenu.transform.SetParent(root.transform);
            settings.transform.SetParent(root.transform);
            level.transform.SetParent(root.transform);

            history = new ScreenNavigationHistory();
            navigator = new ScreenNavigator(history, new ScreenRegistry(mainMenu, settings, level));
        }

        [TearDown]
        public void TearDown() => UnityEngine.Object.DestroyImmediate(root);

        [Test]
        public void GenericNavigationPreservesHistoryAndShowsOnlyCurrentScreen()
        {
            Assert.IsTrue(navigator.ResetTo<MainMenuView>());
            Assert.IsTrue(mainMenu.gameObject.activeSelf);
            Assert.IsTrue(navigator.Push<SettingsView>());
            Assert.IsTrue(settings.gameObject.activeSelf);
            Assert.IsFalse(mainMenu.gameObject.activeSelf);
            Assert.IsFalse(navigator.Push<MainMenuView>());
            Assert.IsTrue(navigator.Replace<LevelView>());
            Assert.IsTrue(level.gameObject.activeSelf);
            Assert.IsTrue(navigator.Pop());
            Assert.IsTrue(mainMenu.gameObject.activeSelf);
            Assert.IsFalse(settings.gameObject.activeSelf);
            Assert.IsFalse(level.gameObject.activeSelf);
            Assert.AreEqual(1, history.Count);
            Assert.AreEqual(typeof(MainMenuView), history.Current);
        }

        [Test]
        public void UnregisteredScreensLeaveCurrentScreenUntouched()
        {
            Assert.IsTrue(navigator.ResetTo<MainMenuView>());
            Assert.IsFalse(navigator.Push<ShopView>());
            Assert.IsFalse(navigator.Replace<ShopView>());
            Assert.IsFalse(navigator.ResetTo<ShopView>());
            Assert.IsTrue(mainMenu.gameObject.activeSelf);
            Assert.AreEqual(1, history.Count);
            Assert.AreEqual(typeof(MainMenuView), history.Current);
        }
    }
}
