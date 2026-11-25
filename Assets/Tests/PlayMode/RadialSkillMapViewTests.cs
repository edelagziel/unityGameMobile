using System.Collections;
using System.Collections.Generic;
using Game.Services.Skills;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Game.ScreenNavigation.Tests
{
    public sealed class RadialSkillMapViewTests : IPrebuildSetup, IPostBuildCleanup
    {
        public void Setup()
        {
#if UNITY_EDITOR
            // The project's ordinary Play shortcut forces Bootstrap. This run needs UTF's scene.
            SessionState.SetString("BattleForge.RadialTest.StartScene",
                AssetDatabase.GetAssetPath(EditorSceneManager.playModeStartScene) ?? "");
            EditorApplication.playModeStateChanged += AllowTestRunnerScene;
#endif
        }

#if UNITY_EDITOR
        private static void AllowTestRunnerScene(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.ExitingEditMode) return;
            EditorApplication.playModeStateChanged -= AllowTestRunnerScene;
            EditorSceneManager.playModeStartScene = null;
        }
#endif

        public void Cleanup()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= AllowTestRunnerScene;
            const string key = "BattleForge.RadialTest.StartScene";
            string path = SessionState.GetString(key, "");
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            SessionState.EraseString(key);
#endif
        }

        [UnityTest]
        public IEnumerator ReciprocalArrowsStayOnTheRimsAndReuseTheirGraphicsWhenOffersChange()
        {
            var tree = ScriptableObject.CreateInstance<SkillTreeDefinition>();
            var canvas = new GameObject("Test canvas", typeof(RectTransform), typeof(Canvas));
            try
            {
                canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                // Both first levels are reachable. Each second level uses the other skill's first.
                JsonUtility.FromJsonOverwrite("{\"id\":\"test\",\"title\":\"Test\",\"skills\":[" +
                    Definition("alpha", "beta") + "," + Definition("beta", "alpha") + "],\"references\":{\"version\":2,\"RefIds\":[{\"rid\":1,\"type\":{\"class\":\"StatModifierEffect\",\"ns\":\"Game.Services.Skills\",\"asm\":\"Services\"},\"data\":{\"stat\":6,\"amount\":1}}]}}", tree);
                var screen = new GameObject("Screen", typeof(RectTransform), typeof(SkillTreeView));
                var rect = (RectTransform)screen.transform;
                rect.SetParent(canvas.transform, false);
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = rect.offsetMax = Vector2.zero;
                var view = screen.GetComponent<SkillTreeView>();
                var owner = new SkillContext("test", "player");
                var offers = new Dictionary<string, SkillOffer>
                {
                    ["alpha"] = new SkillOffer(0, 10, SkillAvailability.Available),
                    ["beta"] = new SkillOffer(0, 10, SkillAvailability.Available)
                };
                view.Render(owner, tree, "alpha", offers, 50, null);
                yield return null;
                var map = view.GetComponentInChildren<MapPanZoomController>();
                var alpha = (RectTransform)map.content.Find("alpha");
                var beta = (RectTransform)map.content.Find("beta");
                var forward = map.content.Find("Connections/alpha to beta").GetComponent<SkillArrowGraphic>();
                var reverse = map.content.Find("Connections/beta to alpha").GetComponent<SkillArrowGraphic>();
                var core = (RectTransform)map.content.Find("Core");
                var spoke = map.content.Find("Connections/Core to alpha").GetComponent<SkillArrowGraphic>();
                Assert.IsTrue(spoke.gameObject.activeInHierarchy);
                Assert.IsTrue(forward.gameObject.activeInHierarchy);
                Assert.IsTrue(reverse.gameObject.activeInHierarchy);
                Assert.IsNotNull(map.content.Find("Core"));
                Assert.AreEqual(2, map.content.GetComponentsInChildren<Button>().Length);
                var later = forward.color;

                offers = new Dictionary<string, SkillOffer>(offers) { ["beta"] = new SkillOffer(1, 10, SkillAvailability.Locked) };
                view.Render(owner, tree, "beta", offers, 40, null);
                Assert.IsFalse(reverse.gameObject.activeSelf);
                Assert.IsTrue(forward.gameObject.activeSelf);
                Assert.AreNotEqual(later, forward.color);
                var unmet = forward.color;
                offers = new Dictionary<string, SkillOffer>(offers) { ["alpha"] = new SkillOffer(1, 10, SkillAvailability.Available) };
                view.Render(owner, tree, "beta", offers, 30, null);
                Assert.AreNotEqual(unmet, forward.color);
                Assert.AreSame(forward.transform, map.content.Find("Connections/alpha to beta"));
                Assert.AreSame(alpha, map.content.Find("alpha"));
                Assert.IsTrue(beta.GetComponent<Outline>().enabled);

                foreach (int ticks in new[] { 2, -4 })
                {
                    map.OnScroll(new PointerEventData(null) { position = map.viewport.position, scrollDelta = ticks * Vector2.up });
                    map.normalizedPosition = new Vector2(.3f, .6f);
                    yield return null;
                    CheckRims(forward.rectTransform, alpha, beta);
                    CheckRims(reverse.rectTransform, beta, alpha);
                    CheckRims(spoke.rectTransform, core, alpha);
                }
                map.Focus();
                Assert.AreEqual(Vector2.zero, map.content.anchoredPosition);

                offers["beta"] = new SkillOffer(2, 0, SkillAvailability.Mastered);
                offers["alpha"] = new SkillOffer(0, 10, SkillAvailability.Available);
                view.Render(owner, tree, "beta", offers, 30, null);
                Assert.AreEqual(unmet, forward.color, "Saved mastery alone cannot satisfy a changed prerequisite.");

#if UNITY_EDITOR
                var previousContent = map.content;
                var data = new SerializedObject(tree);
                data.FindProperty("ringSpacing").floatValue = 700;
                data.ApplyModifiedPropertiesWithoutUndo();
                view.Render(owner, tree, "beta", offers, 30, null);
                Assert.AreNotSame(previousContent, map.content);
                Assert.That(((RectTransform)map.content.Find("alpha")).anchoredPosition.magnitude,
                    Is.EqualTo(700).Within(.02f));
#endif
            }
            finally
            {
                Object.Destroy(canvas);
                Object.Destroy(tree);
            }
        }

        private static string Definition(string id, string other) =>
            "{\"id\":\"" + id + "\",\"title\":\"" + id + "\",\"requirements\":[],\"levels\":[" +
            "{\"fixedCash\":10,\"effects\":[{\"rid\":1}],\"requirements\":[]},{\"fixedCash\":10,\"effects\":[{\"rid\":1}],\"requirements\":[{\"skillId\":\"" + other + "\",\"requiredLevel\":1}]}]}";

        private static void CheckRims(RectTransform edge, RectTransform source, RectTransform target)
        {
            var arrow = edge.GetComponent<SkillArrowGraphic>();
            float startRadius = source.InverseTransformPoint(edge.TransformPoint(arrow.StartPoint)).magnitude;
            float endRadius = target.InverseTransformPoint(edge.TransformPoint(arrow.EndPoint)).magnitude;
            Assert.That(startRadius, Is.EqualTo(source.rect.width / 2).Within(.02f));
            Assert.That(endRadius, Is.EqualTo(target.rect.width / 2).Within(.02f));
        }
    }
}
