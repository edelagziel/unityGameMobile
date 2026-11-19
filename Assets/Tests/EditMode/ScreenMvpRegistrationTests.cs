using System;
using Game.ScreenNavigation;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ScreenNavigation.Tests
{
    public sealed class ScreenMvpRegistrationTests
    {
        private GameObject root;
        private ScreenNavigationLifetimeScope scope;

        [TearDown]
        public void TearDown()
        {
            // Build() queued the controllers' Start on the player loop; disposing the container cancels it,
            // otherwise Start runs later on destroyed views (MissingReferenceException in the Console).
            if (scope != null) scope.DisposeCore();
            if (root != null) UnityEngine.Object.DestroyImmediate(root);
        }

        [Test]
        public void ScopeResolvesEveryScreenModelAndController()
        {
            root = new GameObject("MVP registration test");
            root.SetActive(false);
            scope = root.AddComponent<ScreenNavigationLifetimeScope>();
            scope.autoRun = false;

            var views = new ScreenView[]
            {
                CreateView<MainMenuView>(),
                CreateView<SettingsView>(),
                CreateView<ShopView>(),
                CreateView<InventoryView>(),
                CreateView<SkillTreeView>(),
                CreateView<RecipesView>(),
                CreateView<LevelView>()
            };
            foreach (var screen in views)
            {
                if (!(screen is TabbedScreenView)) continue;
                var button = CreateView<Button>();
                var highlight = new GameObject("Highlight");
                highlight.transform.SetParent(button.transform);
                var fields = new SerializedObject(screen);
                fields.FindProperty("tabButton").objectReferenceValue = button;
                fields.FindProperty("tabHighlight").objectReferenceValue = highlight;
                fields.ApplyModifiedPropertiesWithoutUndo();
            }
            var serialized = new SerializedObject(scope);
            var screens = serialized.FindProperty("screens");
            screens.arraySize = views.Length;
            for (var i = 0; i < views.Length; i++)
                screens.GetArrayElementAtIndex(i).objectReferenceValue = views[i];
            serialized.ApplyModifiedPropertiesWithoutUndo();

            scope.Build();

            var requiredTypes = new[]
            {
                typeof(MainMenuModel), typeof(MainMenuController),
                typeof(SettingsModel), typeof(SettingsController),
                typeof(ShopModel), typeof(ShopController),
                typeof(InventoryModel), typeof(InventoryController),
                typeof(SkillTreeModel), typeof(SkillTreeController),
                typeof(RecipesModel), typeof(RecipesController),
                typeof(LevelModel), typeof(LevelController)
            };
            foreach (Type type in requiredTypes)
                Assert.IsNotNull(scope.Container.Resolve(type), type.Name);
        }

        private TView CreateView<TView>() where TView : Component
        {
            var gameObject = new GameObject(typeof(TView).Name);
            gameObject.transform.SetParent(root.transform);
            return gameObject.AddComponent<TView>();
        }
    }
}
