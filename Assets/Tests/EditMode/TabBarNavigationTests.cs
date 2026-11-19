using System;
using System.Reflection;
using Game.ScreenNavigation;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VContainer.Unity;

namespace Game.ScreenNavigation.Tests
{
    public sealed class TabBarNavigationTests
    {
        private GameObject root;
        private Button[] buttons;
        private GameObject[] highlights;
        private Button settingsButton;
        private Button playButton;
        private IStartable[] controllers;
        private ShopController shopController;
        private RecordingNavigator navigator;

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("TabBar navigation tests");
            root.SetActive(false);
            buttons = new Button[5];
            highlights = new GameObject[5];
            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i] = CreateComponent<Button>($"Tab {i}");
                highlights[i] = new GameObject($"Highlight {i}");
                highlights[i].transform.SetParent(buttons[i].transform);
                highlights[i].SetActive(false);
            }

            var mainMenu = CreateComponent<MainMenuView>("MainMenu");
            settingsButton = CreateComponent<Button>("Settings", mainMenu.transform);
            playButton = CreateComponent<Button>("Play", mainMenu.transform);
            Assign(mainMenu, "settingsButton", settingsButton);
            Assign(mainMenu, "playButton", playButton);
            var shop = CreateComponent<ShopView>("Shop");
            var inventory = CreateComponent<InventoryView>("Inventory");
            var skillTree = CreateComponent<SkillTreeView>("SkillTree");
            var recipes = CreateComponent<RecipesView>("Recipes");

            var views = new TabbedScreenView[] { shop, inventory, mainMenu, skillTree, recipes };
            var selection = new TabHighlightSelection();
            for (var i = 0; i < views.Length; i++)
            {
                Assign(views[i], "tabButton", buttons[i]);
                Assign(views[i], "tabHighlight", highlights[i]);
                views[i].InitializeTab(selection);
                views[i].gameObject.SetActive(views[i] == mainMenu);
            }

            var settings = CreateComponent<SettingsView>("Settings screen");
            var level = CreateComponent<LevelView>("Level screen");
            Assign(settings, "backButton", CreateComponent<Button>("Back", settings.transform));
            Assign(level, "backButton", CreateComponent<Button>("Back", level.transform));
            var registry = new ScreenRegistry(mainMenu, shop, inventory, skillTree, recipes, settings, level);
            var screenNavigator = new ScreenNavigator(new ScreenNavigationHistory(), registry);
            navigator = new RecordingNavigator(screenNavigator);
            shopController = new ShopController(shop, new ShopModel(), navigator);
            controllers = new IStartable[]
            {
                new MainMenuController(mainMenu, new MainMenuModel(), navigator),
                shopController,
                new InventoryController(inventory, new InventoryModel(), navigator),
                new SkillTreeController(skillTree, new SkillTreeModel(), navigator),
                new RecipesController(recipes, new RecipesModel(), navigator)
            };
            root.SetActive(true);
            // Wire MainMenu's Settings and Play buttons as Unity does in Play Mode.
            typeof(MainMenuView).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(mainMenu, null);
            foreach (var controller in controllers) controller.Start();
            new ScreenNavigationInitializer(screenNavigator).Start();
        }

        [TearDown]
        public void TearDown()
        {
            if (controllers != null)
                foreach (var controller in controllers) (controller as IDisposable)?.Dispose();
            if (root != null) UnityEngine.Object.DestroyImmediate(root);
        }

        [Test]
        public void EachButtonInvokesOnlyItsDestinationController()
        {
            var destinations = new[]
            {
                typeof(ShopView), typeof(InventoryView), typeof(MainMenuView),
                typeof(SkillTreeView), typeof(RecipesView)
            };
            Assert.IsTrue(highlights[2].activeSelf);
            for (var destination = 0; destination < buttons.Length; destination++)
            {
                var previousRequests = navigator.ResetCount;
                buttons[destination].onClick.Invoke();
                Assert.AreEqual(previousRequests + 1, navigator.ResetCount);
                Assert.AreEqual(destinations[destination], navigator.LastRequested);
                for (var highlight = 0; highlight < highlights.Length; highlight++)
                    Assert.AreEqual(highlight == destination, highlights[highlight].activeSelf);
            }
        }

        [Test]
        public void FailedNavigationKeepsThePreviousHighlight()
        {
            navigator.AcceptNavigation = false;
            buttons[0].onClick.Invoke();
            Assert.AreEqual(1, navigator.ResetCount);
            Assert.IsTrue(highlights[2].activeSelf);
            Assert.IsFalse(highlights[0].activeSelf);
        }

        [Test]
        public void SettingsAndLevelNavigationKeepThePrimaryHighlight()
        {
            settingsButton.onClick.Invoke();
            Assert.AreEqual(typeof(SettingsView), navigator.LastPushed);
            Assert.IsTrue(highlights[2].activeSelf);
            Assert.IsTrue(navigator.Pop());
            playButton.onClick.Invoke();
            Assert.AreEqual(typeof(LevelView), navigator.LastPushed);
            Assert.IsTrue(highlights[2].activeSelf);
        }

        [Test]
        public void DirectNavigationAndBackHighlightTheShownScreen()
        {
            Assert.IsTrue(navigator.Push<ShopView>());
            Assert.IsTrue(highlights[0].activeSelf);
            Assert.IsFalse(highlights[2].activeSelf);
            Assert.IsTrue(navigator.Replace<InventoryView>());
            Assert.IsTrue(highlights[1].activeSelf);
            Assert.IsFalse(highlights[0].activeSelf);
            Assert.IsTrue(navigator.Pop());
            Assert.IsTrue(highlights[2].activeSelf);
            Assert.IsFalse(highlights[1].activeSelf);
        }

        [Test]
        public void RepeatedStartAndDisposalDoNotDuplicateListeners()
        {
            shopController.Start();
            buttons[0].onClick.Invoke();
            Assert.AreEqual(1, navigator.ResetCount);

            shopController.Dispose();
            buttons[0].onClick.Invoke();
            Assert.AreEqual(1, navigator.ResetCount);
            buttons[1].onClick.Invoke();
            Assert.AreEqual(2, navigator.ResetCount);
        }

        [Test]
        public void DisposingShopLeavesOtherButtonListenersConnected()
        {
            var unrelatedClicks = 0;
            buttons[0].onClick.AddListener(() => unrelatedClicks++);
            shopController.Dispose();
            buttons[0].onClick.Invoke();
            Assert.AreEqual(1, unrelatedClicks);
            Assert.AreEqual(0, navigator.ResetCount);
        }

        private T CreateComponent<T>(string name, Transform parent = null) where T : Component
        {
            var gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent == null ? root.transform : parent);
            return gameObject.AddComponent<T>();
        }

        private static void Assign(UnityEngine.Object target, string field, UnityEngine.Object value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(field).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private sealed class RecordingNavigator : IScreenNavigator
        {
            private readonly IScreenNavigator inner;

            public RecordingNavigator(IScreenNavigator inner) => this.inner = inner;

            public Type LastRequested { get; private set; }
            public Type LastPushed { get; private set; }
            public int ResetCount { get; private set; }
            public bool AcceptNavigation { get; set; } = true;

            public bool Push<TView>() where TView : ScreenView
            {
                LastPushed = typeof(TView);
                return inner.Push<TView>();
            }

            public bool Pop() => inner.Pop();
            public bool Replace<TView>() where TView : ScreenView => inner.Replace<TView>();
            public bool ResetTo<TView>() where TView : ScreenView
            {
                LastRequested = typeof(TView);
                ResetCount++;
                return AcceptNavigation && inner.ResetTo<TView>();
            }
        }
    }
}
