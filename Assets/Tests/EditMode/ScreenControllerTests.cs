using System;
using System.Reflection;
using Game.ScreenNavigation;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ScreenNavigation.Tests
{
    public sealed class ScreenControllerTests
    {
        private GameObject root;
        private MainMenuView mainMenu;
        private SettingsView settings;
        private LevelView level;
        private Button settingsButton;
        private Button playButton;
        private Button settingsBackButton;
        private Button levelBackButton;
        private Toggle soundToggle;
        private RecordingNavigator navigator;

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("Screen controller tests");
            root.SetActive(false);

            mainMenu = CreateView<MainMenuView>("MainMenu");
            var battleButton = CreateButton(root.transform, "Battle");
            var battleHighlight = new GameObject("Battle highlight");
            battleHighlight.transform.SetParent(battleButton.transform);
            Assign(mainMenu, "tabButton", battleButton);
            Assign(mainMenu, "tabHighlight", battleHighlight);
            mainMenu.InitializeTab(new TabHighlightSelection());
            settingsButton = CreateButton(mainMenu.transform, "Settings");
            playButton = CreateButton(mainMenu.transform, "Play");
            Assign(mainMenu, "settingsButton", settingsButton);
            Assign(mainMenu, "playButton", playButton);

            settings = CreateView<SettingsView>("Settings");
            settingsBackButton = CreateButton(settings.transform, "Back");
            soundToggle = new GameObject("Sound").AddComponent<Toggle>();
            soundToggle.transform.SetParent(settings.transform);
            Assign(settings, "backButton", settingsBackButton);
            Assign(settings, "soundToggle", soundToggle);

            level = CreateView<LevelView>("Level");
            levelBackButton = CreateButton(level.transform, "Back");
            Assign(level, "backButton", levelBackButton);

            navigator = new RecordingNavigator();
            root.SetActive(true);
            // Edit Mode does not run the scene Views' normal Play Mode startup.
            InvokeAwake(mainMenu);
            InvokeAwake(settings);
            InvokeAwake(level);
        }

        private static void InvokeAwake(ScreenView view) =>
            view.GetType().GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(view, null);

        [TearDown]
        public void TearDown() => UnityEngine.Object.DestroyImmediate(root);

        [Test]
        public void MainMenuRoutesItsButtonsAndUnsubscribes()
        {
            var controller = new MainMenuController(mainMenu, new MainMenuModel(), navigator);
            controller.Start();

            settingsButton.onClick.Invoke();
            Assert.AreEqual(typeof(SettingsView), navigator.LastPushed);
            playButton.onClick.Invoke();
            Assert.AreEqual(typeof(LevelView), navigator.LastPushed);
            Assert.AreEqual(2, navigator.PushCount);
            Assert.AreEqual(0, navigator.PopCount);

            controller.Dispose();
            playButton.onClick.Invoke();
            Assert.AreEqual(2, navigator.PushCount);
        }

        [Test]
        public void SettingsOwnsSoundStateAndBackAction()
        {
            var model = new SettingsModel();
            model.SetSoundEnabled(false);
            var controller = new SettingsController(settings, navigator, model);
            controller.Start();

            Assert.IsFalse(soundToggle.isOn);
            soundToggle.isOn = true;
            Assert.IsTrue(model.SoundEnabled);
            settingsBackButton.onClick.Invoke();
            Assert.AreEqual(1, navigator.PopCount);
            Assert.AreEqual(0, navigator.PushCount);

            controller.Dispose();
            soundToggle.isOn = false;
            settingsBackButton.onClick.Invoke();
            Assert.IsTrue(model.SoundEnabled);
            Assert.AreEqual(1, navigator.PopCount);
        }

        [Test]
        public void LevelOwnsBackActionAndUnsubscribes()
        {
            var controller = new LevelController(level, new LevelModel(), navigator);
            controller.Start();

            levelBackButton.onClick.Invoke();
            Assert.AreEqual(1, navigator.PopCount);
            Assert.AreEqual(0, navigator.PushCount);

            controller.Dispose();
            levelBackButton.onClick.Invoke();
            Assert.AreEqual(1, navigator.PopCount);
        }

        private TView CreateView<TView>(string name) where TView : ScreenView
        {
            var gameObject = new GameObject(name);
            gameObject.transform.SetParent(root.transform);
            return gameObject.AddComponent<TView>();
        }

        private static Button CreateButton(Transform parent, string name)
        {
            var gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent);
            return gameObject.AddComponent<Button>();
        }

        private static void Assign(UnityEngine.Object target, string field, UnityEngine.Object value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(field).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private sealed class RecordingNavigator : IScreenNavigator
        {
            public Type LastPushed { get; private set; }
            public int PushCount { get; private set; }
            public int PopCount { get; private set; }

            public bool Push<TView>() where TView : ScreenView
            {
                LastPushed = typeof(TView);
                PushCount++;
                return true;
            }

            public bool Pop()
            {
                PopCount++;
                return true;
            }

            public bool Replace<TView>() where TView : ScreenView => throw new NotSupportedException();
            public bool ResetTo<TView>() where TView : ScreenView => throw new NotSupportedException();
        }
    }
}
