using System;
using System.Collections.Generic;
using System.Linq;
using Game.Services.Config;
using Game.Services.Skills;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.ScreenNavigation.Tests
{
    public sealed class SkillMapLayoutTests
    {
        private readonly List<SkillTreeDefinition> fixtures = new();

        [TearDown]
        public void TearDown()
        {
            foreach (var tree in fixtures) UnityEngine.Object.DestroyImmediate(tree);
            fixtures.Clear();
        }

        [Test]
        public void LongestParentPathSetsDepthAcrossBranchesAndComponents()
        {
            var tree = Tree(Node("army"), Node("economy"), Node("isolated"),
                Node("forge", "army"), Node("cards", "forge", "economy"), Node("deck", "cards"));
            var layout = new SkillMapLayout(tree);
            AssertHierarchy(tree, layout);
            foreach (string id in new[] { "army", "economy", "isolated" }) Assert.AreEqual(1, layout.Layer(id));
            Assert.AreEqual(2, layout.Layer("forge"));
            Assert.AreEqual(3, layout.Layer("cards"));
            Assert.AreEqual(4, layout.Layer("deck"));
            Assert.AreEqual(4, layout.Connections.Count);
            foreach (var edge in layout.Connections)
                Assert.Less(layout.Position(edge.Source).magnitude, layout.Position(edge.Target).magnitude);
        }

        [Test]
        public void StableIdsAndDependenciesDeterminePositionsAcrossAuthoringOrders()
        {
            var first = new SkillMapLayout(Tree(Node("start"), Node("alpha", "start"), Node("beta", "start"),
                Node("x", "alpha"), Node("y", "beta"), Node("joint", "alpha", "beta")));
            var second = new SkillMapLayout(Tree(Node("joint", "beta", "alpha"), Node("y", "beta"),
                Node("x", "alpha"), Node("beta", "start"), Node("alpha", "start"), Node("start")));
            Assert.AreEqual(1, first.Layer("start"));
            Assert.IsNull(first.Edges.Single(e => e.Target == "start").Source);
            foreach (string id in new[] { "start", "alpha", "beta", "x", "y", "joint" })
                Assert.AreEqual(first.Position(id), second.Position(id));
            Assert.Less(Vector2.Angle(first.Position("x"), first.Position("alpha")),
                Vector2.Angle(first.Position("x"), first.Position("beta")));
        }

        [TestCase(1)]
        [TestCase(12)]
        [TestCase(96)]
        public void BroadRingsSeparateEveryFootprintAndFitWithinCalculatedBounds(int count)
        {
            var children = Enumerable.Range(0, count).Select(i => Node("child" + i, "root"));
            var tree = Tree(children.Prepend(Node("root")).ToArray());
            var layout = new SkillMapLayout(tree);
            foreach (var skill in tree.Skills)
            {
                var position = layout.Position(skill.Id);
                Assert.Less(Mathf.Abs(position.x) + SkillMapLayout.FootprintRadius, layout.Size.x / 2);
                Assert.Less(Mathf.Abs(position.y) + SkillMapLayout.FootprintRadius, layout.Size.y / 2);
                foreach (var other in tree.Skills.Where(s => string.CompareOrdinal(s.Id, skill.Id) > 0))
                    Assert.GreaterOrEqual(Vector2.Distance(position, layout.Position(other.Id)),
                        2 * SkillMapLayout.FootprintRadius + tree.NodeSpacing - .02f);
            }
            if (count > 1) Assert.Greater(layout.Size.x, new SkillMapLayout(Tree(Node("root"))).Size.x);
        }

        [Test]
        public void ValidLevelCycleHasOneEntryAndKeepsDirectionalThresholds()
        {
            string a = Skill("a", "", Level(), Level(Requirement("b", 1)));
            string b = Skill("b", Requirement("a", 1), Level(), Level(Requirement("a", 2)));
            var tree = Tree(a, b, Node("leaf", "b"));
            Assert.DoesNotThrow(() => new SkillCatalog(new[] { tree }, new PriceCurve(1, 1, 3)));
            var layout = new SkillMapLayout(tree);
            AssertHierarchy(tree, layout);
            Assert.AreEqual(1, layout.Layer("a"));
            Assert.AreEqual(2, layout.Layer("b"));
            Assert.AreEqual(3, layout.Layer("leaf"));
            Assert.AreEqual(3, layout.Connections.Count);
            var ab = layout.Connections.Single(e => e.Source == "a" && e.Target == "b");
            var ba = layout.Connections.Single(e => e.Source == "b" && e.Target == "a");
            Assert.AreEqual(1, ab.RequiredFor(1));
            Assert.AreEqual(2, ab.RequiredFor(2));
            Assert.AreEqual(0, ba.RequiredFor(1));
            Assert.AreEqual(1, ba.RequiredFor(2));
        }

        [Test]
        public void GlobalCrossBranchArrowClearsTheCoreAndUnrelatedRoot()
        {
            var tree = AssetDatabase.LoadAssetAtPath<SkillTreeDefinition>("Assets/Config/Skills/global.asset");
            var layout = new SkillMapLayout(tree);
            var route = layout.Edges.Single(e => e.Source == "trade" && e.Target == "reserve");
            Assert.IsFalse(route.Primary);
            AssertHierarchy(tree, layout);
        }

        private static void AssertHierarchy(SkillTreeDefinition tree, SkillMapLayout layout)
        {
            var all = layout.Edges;
            var primary = all.Where(e => e.Primary).ToDictionary(e => e.Target);
            Assert.AreEqual(tree.Skills.Count, primary.Count);
            Assert.AreEqual(all.Count, all.Select(e => (e.Source, e.Target)).Distinct().Count());
            CollectionAssert.AreEquivalent(layout.Connections, all.Where(e => e.Dependency != null).Select(e => e.Dependency));
            foreach (var skill in tree.Skills)
            {
                if (!layout.Connections.Any(e => e.Target == skill.Id)) Assert.IsNull(primary[skill.Id].Source);
                string id = skill.Id;
                int layer = layout.Layer(id);
                while (id != null)
                {
                    Assert.AreEqual(layer--, layout.Layer(id));
                    id = primary[id].Source;
                }
                Assert.AreEqual(0, layer);
            }
            foreach (var edge in all)
            for (int i = 1; i < edge.Points.Count; i++)
            {
                var start = edge.Points[i - 1];
                var segment = edge.Points[i] - start;
                foreach (string id in tree.Skills.Select(s => s.Id).Append(null))
                {
                    if (id == edge.Source || id == edge.Target) continue;
                    var obstacle = id == null ? Vector2.zero : layout.Position(id);
                    float along = Mathf.Clamp01(Vector2.Dot(obstacle - start, segment) / segment.sqrMagnitude);
                    float radius = id == null ? SkillMapLayout.CoreRadius : SkillMapLayout.NodeRadius;
                    Assert.Greater(Vector2.Distance(obstacle, start + along * segment), radius,
                        $"{edge.Source ?? "Core"} to {edge.Target} crosses {id ?? "Core"}");
                }
            }
        }

        [TestCase("global", "drill", "deck", 4)]
        [TestCase("swordsman", "health", "veteran", 3)]
        [TestCase("archer", "damage", "volley", 2)]
        public void ShippedAssetsUseTheirOwnDependencyGraph(string id, string root, string leaf, int depth)
        {
            var tree = AssetDatabase.LoadAssetAtPath<SkillTreeDefinition>($"Assets/Config/Skills/{id}.asset");
            var layout = new SkillMapLayout(tree);
            Assert.AreEqual(depth, layout.Layer(leaf));
            Assert.AreEqual(1, layout.Layer(root));
            AssertHierarchy(tree, layout);
            Assert.Greater(layout.Position(leaf).magnitude, layout.Position(root).magnitude);
        }

        private SkillTreeDefinition Tree(params string[] skills)
        {
            var tree = ScriptableObject.CreateInstance<SkillTreeDefinition>();
            fixtures.Add(tree);
            JsonUtility.FromJsonOverwrite("{\"id\":\"test\",\"title\":\"Test\",\"skills\":[" +
                string.Join(",", skills) + "],\"references\":{\"version\":2,\"RefIds\":[{\"rid\":1,\"type\":{\"class\":\"StatModifierEffect\",\"ns\":\"Game.Services.Skills\",\"asm\":\"Services\"},\"data\":{\"stat\":6,\"amount\":1}}]}}", tree);
            return tree;
        }

        private static string Node(string id, params string[] sources) =>
            Skill(id, string.Join(",", sources.Select(s => Requirement(s, 1))), Level());

        private static string Requirement(string id, int level) => $"{{\"skillId\":\"{id}\",\"requiredLevel\":{level}}}";

        private static string Level(params string[] gates) =>
            "{\"fixedCash\":1,\"effects\":[{\"rid\":1}],\"requirements\":[" +
            string.Join(",", gates) + "]}";

        private static string Skill(string id, string gates, params string[] levels) =>
            $"{{\"id\":\"{id}\",\"title\":\"{id}\",\"requirements\":[{gates}],\"levels\":[{string.Join(",", levels)}]}}";
    }
}
