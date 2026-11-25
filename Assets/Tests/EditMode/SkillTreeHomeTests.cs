using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Game.Services.Config;
using Game.Services.Save;
using Game.Services.Skills;
using Game.Services.Wallet;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;
using VContainer;

namespace Game.ScreenNavigation.Tests
{
    public sealed class SkillTreeHomeTests
    {
        [UnityTest]
        public IEnumerator HomeTabBuildsGraphAndButtonsPurchaseTheDisplayedLevel()
        {
            yield return new EnterPlayMode();
            for (int i = 0; i < 300 && SceneManager.GetActiveScene().name != "Home"; i++) yield return null;
            Assert.AreEqual("Home", SceneManager.GetActiveScene().name);
            yield return null;
            yield return null;
            var scope = Object.FindAnyObjectByType<ScreenNavigationLifetimeScope>();
            var container = scope.Container;
            var tab = GameObject.Find("Tab_SkillTree").GetComponent<Button>();
            tab.onClick.Invoke();
            var view = container.Resolve<SkillTreeView>();
            Assert.IsTrue(view.Visible);
            Assert.IsNotNull(FindButton(view, "Army Drills"));
            Assert.IsNotNull(view.GetComponentsInChildren<RectTransform>().FirstOrDefault(r => r.name == "forge to reserve"));

            // Exercise purchases on the shipped view without touching the developer's saved Cash.
            container.Resolve<SkillTreeController>().Dispose();
            var wallet = new WalletService(new MemorySave());
            wallet.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
            var skills = new SkillService(container.Resolve<IConfigService>(), wallet);
            skills.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
            using (var controller = new SkillTreeController(view, new SkillTreeModel(skills), container.Resolve<IScreenNavigator>(), wallet))
            {
                controller.Start();
                yield return null;
                Canvas.ForceUpdateCanvases();
                var map = view.GetComponentInChildren<MapPanZoomController>();
                var originalContent = map.content;
                var drill = FindButton(view, "Army Drills");
                var forge = FindButton(view, "Field Forge");
                var edge = map.content.Find("Connections/forge to reserve") as RectTransform;
                var reserve = FindButton(view, "Reserve Cards");
                Assert.IsNotNull(map.content.Find("Core"));
                Assert.IsNull(map.content.Find("Core").GetComponent<Button>());
                Assert.IsTrue(drill.GetComponent<Outline>().enabled);
                CheckVisibleHierarchy(map, skills.Trees.Single(t => t.Id == "global"));
                AssertReceivesPointer(drill);
                var overview = map.content.localScale;
                Assert.LessOrEqual(map.content.rect.width * overview.x, map.viewport.rect.width + .01f);
                Assert.LessOrEqual(map.content.rect.height * overview.y, map.viewport.rect.height + .01f);
                ZoomToNormal(map);
                var cursor = map.viewport.TransformPoint(map.viewport.rect.size * .12f);
                Vector3 anchor = map.content.InverseTransformPoint(cursor);
                Wheel(map, cursor, 2);
                Assert.Greater(map.content.localScale.x, 1);
                Assert.That(Vector3.Distance(map.content.TransformPoint(anchor), cursor), Is.LessThan(.01f));
                AssertLineConnects(edge, forge.transform, reserve.transform);
                map.normalizedPosition = new Vector2(.65f, .4f);
                var pose = map.content.anchoredPosition;
                var scale = map.content.localScale;
                forge.onClick.Invoke();
                Assert.IsTrue(forge.GetComponent<Outline>().enabled);
                Assert.IsFalse(drill.GetComponent<Outline>().enabled);
                StringAssert.Contains("Requires: Army Drills 0/1", view.transform.Find("Skill Tree/Details/Label").GetComponent<TMP_Text>().text);
                drill.onClick.Invoke();
                Assert.IsFalse(FindButton(view, "Purchase").interactable);
                wallet.Add(100);
                Assert.IsTrue(FindButton(view, "Purchase").interactable);
                Assert.AreEqual(pose, map.content.anchoredPosition);
                var drillEdge = map.content.Find("Connections/drill to forge").GetComponent<SkillArrowGraphic>();
                var unmet = drillEdge.color;
                FindButton(view, "Purchase").onClick.Invoke();
                Assert.AreNotEqual(unmet, drillEdge.color);
                Assert.AreEqual(1, skills.Inspect(new SkillContext("global", "player"), "drill").Current);
                StringAssert.Contains("1/2", drill.transform.Find("Level/Label").GetComponent<TMP_Text>().text);
                forge.onClick.Invoke();
                StringAssert.Contains("Insufficient Cash", view.transform.Find("Skill Tree/Status/Label").GetComponent<TMP_Text>().text);
                drill.onClick.Invoke();
                Assert.AreSame(drill, FindButton(view, "Army Drills"));
                Assert.AreSame(forge, FindButton(view, "Field Forge"));
                Assert.AreSame(edge, map.content.Find("Connections/forge to reserve"));
                Assert.AreSame(originalContent, map.content);
                Assert.AreEqual(pose, map.content.anchoredPosition);
                Assert.AreEqual(scale, map.content.localScale);
                AssertLineConnects(edge, forge.transform, reserve.transform);
                Wheel(map, cursor, 100);
                Assert.AreEqual(2, map.content.localScale.x);
                Wheel(map, cursor, -100);
                Assert.AreEqual(Mathf.Min(.45f, overview.x), map.content.localScale.x);
                FindButton(view, "Center").onClick.Invoke();
                Assert.AreEqual(Vector2.zero, map.content.anchoredPosition);
                Assert.AreEqual(overview, map.content.localScale);
                wallet.Add(100);
                FindButton(view, "Tree").onClick.Invoke();
                Assert.AreNotSame(originalContent, map.content);
                Assert.IsFalse(originalContent.gameObject.activeSelf);
                CheckVisibleHierarchy(map, skills.Trees.Single(t => t.Id == "swordsman"));
                FindButton(view, "Sword Training").onClick.Invoke();
                var buy = FindButton(view, "Purchase");
                yield return null;
                AssertReceivesPointer(buy);
                buy.onClick.Invoke();
                buy.onClick.Invoke();
                Assert.AreEqual(50, wallet.Cash);
                Assert.IsTrue(view.GetComponentsInChildren<TMP_Text>().Any(t => t.text.Contains("Cash  50")));
                Assert.AreEqual(1, skills.Inspect(new SkillContext("swordsman", "type.swordsman"), "damage").Current);
                var swordNode = FindButton(view, "Sword Training");
                Assert.IsTrue(controller.Open(new SkillContext("swordsman", "card.2")));
                swordNode.onClick.Invoke();
                Assert.AreSame(swordNode, FindButton(view, "Sword Training"));
                StringAssert.Contains("0/2", swordNode.transform.Find("Level/Label").GetComponent<TMP_Text>().text);
                FindButton(view, "Tree").onClick.Invoke();
                Assert.IsNotNull(FindButton(view, "Volley"));
                CheckVisibleHierarchy(map, skills.Trees.Single(t => t.Id == "archer"));
                Assert.IsNull(map.content.Find("Sword Training"));
                FindButton(view, "Fletching").onClick.Invoke();
                Assert.IsTrue(FindButton(view, "Fletching").GetComponent<Outline>().enabled);
            }
            yield return new ExitPlayMode();
        }

        [UnityTest]
        public IEnumerator HomeInputModuleRoutesDragWheelPinchAndTap()
        {
            yield return new EnterPlayMode();
            for (int i = 0; i < 300 && SceneManager.GetActiveScene().name != "Home"; i++) yield return null;
            yield return null;
            var scope = Object.FindAnyObjectByType<ScreenNavigationLifetimeScope>();
            var view = scope.Container.Resolve<SkillTreeView>();
            GameObject.Find("Tab_SkillTree").GetComponent<Button>().onClick.Invoke();
            scope.Container.Resolve<SkillTreeController>().Dispose();
            var wallet = new WalletService(new MemorySave());
            wallet.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
            wallet.Add(500);
            var skills = new SkillService(scope.Container.Resolve<IConfigService>(), wallet);
            skills.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
            using (var controller = new SkillTreeController(view, new SkillTreeModel(skills),
                scope.Container.Resolve<IScreenNavigator>(), wallet))
            {
                controller.Start();
                yield return null;
                var module = EventSystem.current.GetComponent<StandaloneInputModule>();
                var input = module.gameObject.AddComponent<SkillMapTestInput>();
                module.inputOverride = input;
                var map = view.GetComponentInChildren<MapPanZoomController>();
                var drill = FindButton(view, "Army Drills");
                FindButton(view, "Trade Routes").onClick.Invoke();
                // Exercise the gesture away from the bounds, with the chosen node under the cursor.
                ZoomToNormal(map);
                map.content.anchoredPosition = -((RectTransform)drill.transform).anchoredPosition;
                var center = ScreenPoint(drill.transform);
                input.Cursor = center;
                input.Press = true;
                yield return null;
                input.Press = false;
                input.Cursor += new Vector2(18, 18);
                yield return null;
                input.Cursor += new Vector2(22, 22);
                yield return null;
                input.Release = true;
                yield return null;
                input.Release = false;
                Assert.Greater(map.content.anchoredPosition.x, -((RectTransform)drill.transform).anchoredPosition.x);
                Assert.Greater(map.content.anchoredPosition.y, 0);
                Assert.IsFalse(drill.GetComponent<Outline>().enabled, "Dragging from a node selected it.");
                input.Wheel = Vector2.up;
                yield return null;
                input.Wheel = Vector2.zero;
                Assert.Greater(map.content.localScale.x, 1);
                FindButton(view, "Center").onClick.Invoke();
                ZoomToNormal(map);
                map.content.anchoredPosition = -((RectTransform)drill.transform).anchoredPosition;
                center = ScreenPoint(drill.transform);

                // One stationary finger remains over the node throughout the pinch.
                var second = center + Vector2.right * 30;
                input.Fingers = new[] { Finger(0, center, TouchPhase.Began), Finger(1, second, TouchPhase.Began) };
                yield return null;
                second += Vector2.right * 15;
                input.Fingers = new[] { Finger(0, center, TouchPhase.Stationary), Finger(1, second, TouchPhase.Moved) };
                yield return null;
                Assert.That(map.content.localScale.x, Is.EqualTo(1.5f).Within(.01f));
                Assert.That(Vector2.Distance(ScreenPoint(drill.transform), center), Is.LessThan(1));
                input.Fingers = new[] { Finger(0, center, TouchPhase.Stationary), Finger(1, second, TouchPhase.Ended) };
                yield return null;
                input.Fingers = new[] { Finger(0, center, TouchPhase.Ended) };
                yield return null;
                Assert.IsFalse(drill.GetComponent<Outline>().enabled, "Releasing the last pinch finger selected a node.");
                input.Fingers = System.Array.Empty<Touch>();
                yield return null;

                var buy = ScreenPoint(FindButton(view, "Purchase").transform);
                input.Fingers = new[] { Finger(0, center, TouchPhase.Began), Finger(1, buy, TouchPhase.Began) };
                yield return null;
                input.Fingers = new[] { Finger(0, center, TouchPhase.Ended), Finger(1, buy, TouchPhase.Ended) };
                yield return null;
                Assert.AreEqual(500, wallet.Cash);
                input.Fingers = System.Array.Empty<Touch>();
                yield return null;
                input.Fingers = new[] { Finger(0, center, TouchPhase.Began) };
                yield return null;
                input.Fingers = new[] { Finger(0, center, TouchPhase.Ended) };
                yield return null;
                Assert.IsTrue(drill.GetComponent<Outline>().enabled, "A fresh tap after a pinch should select.");
                input.Fingers = System.Array.Empty<Touch>();
                yield return null;
                input.Cursor = buy;
                input.Press = true;
                yield return null;
                input.Press = false;
                FindButton(view, "Trade Routes").onClick.Invoke();
                input.Release = true;
                yield return null;
                input.Release = false;
                Assert.AreEqual(500, wallet.Cash, "A held press bought a different skill after its quote changed.");
                input.Press = true;
                yield return null;
                input.Press = false;
                input.Release = true;
                yield return null;
                Assert.AreEqual(450, wallet.Cash, "A new press should buy the currently quoted Trade Routes level.");
                module.inputOverride = null;
            }
            yield return new ExitPlayMode();
        }

        private static Touch Finger(int id, Vector2 position, TouchPhase phase) =>
            new Touch { fingerId = id, position = position, phase = phase, type = TouchType.Direct };

        private static Vector2 ScreenPoint(Transform target) =>
            RectTransformUtility.WorldToScreenPoint(target.GetComponentInParent<Canvas>().worldCamera, target.position);

        private static void ZoomToNormal(MapPanZoomController map) =>
            Wheel(map, map.viewport.position, Mathf.Log(1 / map.content.localScale.x) / Mathf.Log(1.15f));

        private static void Wheel(MapPanZoomController map, Vector3 worldPoint, float ticks)
        {
            var camera = map.GetComponentInParent<Canvas>().worldCamera;
            var pointer = new PointerEventData(EventSystem.current)
            {
                position = RectTransformUtility.WorldToScreenPoint(camera, worldPoint),
                scrollDelta = Vector2.up * ticks,
                pointerCurrentRaycast = new RaycastResult { module = map.GetComponentInParent<GraphicRaycaster>() }
            };
            ExecuteEvents.ExecuteHierarchy(map.gameObject, pointer, ExecuteEvents.scrollHandler);
        }

        private static void CheckVisibleHierarchy(MapPanZoomController map, SkillTreeDefinition tree)
        {
            var layout = new SkillMapLayout(tree);
            var skills = tree.Skills.ToDictionary(s => s.Id);
            var core = map.content.Find("Core");
            Assert.IsNull(core.GetComponent<Button>());
            var strokes = map.content.Find("Connections");
            Assert.AreEqual(layout.Edges.Count, strokes.childCount);
            foreach (var edge in layout.Edges.Where(e => e.Primary))
            {
                var arrow = strokes.Find((edge.Source ?? "Core") + " to " + edge.Target) as RectTransform;
                Assert.IsTrue(arrow.gameObject.activeInHierarchy);
                var from = edge.Source == null ? core : map.content.Find(skills[edge.Source].Title);
                AssertLineConnects(arrow, from, map.content.Find(skills[edge.Target].Title));
            }
        }

        private static void AssertLineConnects(RectTransform edge, Transform from, Transform to)
        {
            var arrow = edge.GetComponent<SkillArrowGraphic>();
            Assert.That(from.InverseTransformPoint(edge.TransformPoint(arrow.StartPoint)).magnitude,
                Is.EqualTo(((RectTransform)from).rect.width / 2).Within(.02f));
            Assert.That(to.InverseTransformPoint(edge.TransformPoint(arrow.EndPoint)).magnitude,
                Is.EqualTo(((RectTransform)to).rect.width / 2).Within(.02f));
        }

        private static void AssertReceivesPointer(Button button)
        {
            Canvas.ForceUpdateCanvases();
            var rect = (RectTransform)button.transform;
            var canvas = button.GetComponentInParent<Canvas>();
            var point = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rect.TransformPoint(rect.rect.center));
            var pointer = new PointerEventData(EventSystem.current) { position = point };
            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, hits);
            Assert.IsNotEmpty(hits);
            Assert.AreSame(button, hits[0].gameObject.GetComponentInParent<Button>(),
                 $"Expected {button.name} at {point}, depth {button.targetGraphic.depth}, canvas {canvas.renderMode}. Hits: " +
                string.Join(", ", hits.Select(h => $"{h.gameObject.name} ({h.depth})")));
        }

        private static Button FindButton(SkillTreeView view, string name) =>
            view.GetComponentsInChildren<Button>().Single(b => b.name == name);

        private sealed class MemorySave : ISaveService
        {
            private readonly Dictionary<string, object> values = new();
            public bool HasKey(string key) => values.ContainsKey(key);
            public void Delete(string key) => values.Remove(key);
            public T Load<T>(string key, T fallback = default) => values.TryGetValue(key, out var v) ? (T)v : fallback;
            public void Save<T>(string key, T value) => values[key] = value;
        }
    }
}
