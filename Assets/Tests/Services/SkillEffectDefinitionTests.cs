using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Game.Services.Config;
using Game.Services.Skills;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Services.Tests
{
    public sealed class SkillEffectDefinitionTests
    {
        [TestCase("global", "drill/1/stat:ArmyHealth:army.health:5|drill/2/stat:ArmyHealth:army.health:8|trade/1/rule:income.rate:1|forge/1/rule:craft.speed:2|reserve/1/content:Card:cards.reserve|formation/1/rule:sample.formation:1|vanguard/1/rule:sample.vanguard:1|market/1/rule:sample.market:1|workshop/1/rule:sample.workshop:1|deck/1/rule:sample.deck:1")]
        [TestCase("archer", "damage/1/stat:UnitDamage:unit.damage:1|damage/2/stat:UnitDamage:unit.damage:3|range/1/stat:UnitRange:unit.range:2|volley/1/ability:ability.volley")]
        [TestCase("swordsman", "health/1/stat:UnitHealth:unit.health:10|health/2/stat:UnitHealth:unit.health:15|damage/1/stat:UnitDamage:unit.damage:2|damage/2/stat:UnitDamage:unit.damage:4|veteran/1/ability:ability.parry|veteran/1/stat:UnitArmor:unit.armor:1|veteran/2/stat:UnitArmor:unit.armor:3")]
        public void ShippedEffectsPreserveSourceOrderAndValues(string id, string expected)
        {
            var tree = Load(id);
            var actual = new List<string>();
            foreach (var skill in tree.Skills)
                for (int i = 0; i < skill.Levels.Count; i++)
                    foreach (var effect in skill.Levels[i].Effects)
                        actual.Add($"{skill.Id}/{i + 1}/{Describe(effect)}");
            CollectionAssert.AreEqual(expected.Split('|'), actual);
        }

        [Test]
        public void InspectorCanAddReplaceRemoveAndReloadNestedEffectsIndependently()
        {
            var path = AssetDatabase.GenerateUniqueAssetPath("Assets/SkillEffectRoundTrip.asset");
            var tree = UnityEngine.Object.Instantiate(Load("global"));
            try
            {
                AssetDatabase.CreateAsset(tree, path);
                var data = new SerializedObject(tree);
                var effects = Effects(data);
                effects.arraySize++;
                var added = effects.GetArrayElementAtIndex(1);
                added.managedReferenceValue = new AbilityUnlockEffect();
                added.FindPropertyRelative("abilityId").stringValue = "test.ability";
                data.ApplyModifiedProperties();
                Assert.AreEqual("army.health", tree.Skills[0].Levels[0].Effects[0].TargetId);

                data.Update();
                effects = Effects(data);
                added = effects.GetArrayElementAtIndex(1);
                added.managedReferenceValue = new ContentUnlockEffect();
                added.FindPropertyRelative("contentType").intValue = (int)SkillContentType.Recipe;
                added.FindPropertyRelative("contentId").stringValue = "test.recipe";
                effects.GetArrayElementAtIndex(0).managedReferenceValue = null;
                effects.DeleteArrayElementAtIndex(0);
                data.ApplyModifiedProperties();
                AssetDatabase.SaveAssetIfDirty(tree);
                Resources.UnloadAsset(tree);
                tree = AssetDatabase.LoadAssetAtPath<SkillTreeDefinition>(path);
                Assert.AreEqual("content:Recipe:test.recipe", Describe(tree.Skills[0].Levels[0].Effects.Single()));
                Assert.AreEqual("stat:ArmyHealth:army.health:8", Describe(tree.Skills[0].Levels[1].Effects.Single()));
            }
            finally { AssetDatabase.DeleteAsset(path); }
        }

        [Test]
        public void InspectorUsesTheEffectListDrawerForNestedLevelEffects()
        {
            var tree = Load("swordsman");
            var data = new SerializedObject(tree);
            var list = Effects(data);
            bool wasExpanded = list.isExpanded;
            try
            {
                list.isExpanded = true;
                float row = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                // Header, type selector, stat, amount, Add button.
                Assert.That(EditorGUI.GetPropertyHeight(list, true), Is.EqualTo(5 * row).Within(.1f));
            }
            finally { list.isExpanded = wasExpanded; }
        }

        [TestCase("missing")]
        [TestCase("unknownStat")]
        [TestCase("nonFinite")]
        [TestCase("invalidContentType")]
        [TestCase("invalidAbilityId")]
        public void CatalogRejectsInvalidEffects(string invalid)
        {
            var tree = UnityEngine.Object.Instantiate(Load("global"));
            try
            {
                var data = new SerializedObject(tree);
                var item = Effects(data).GetArrayElementAtIndex(0);
                switch (invalid)
                {
                    case "missing": item.managedReferenceValue = null; break;
                    case "unknownStat": item.FindPropertyRelative("stat").intValue = 999; break;
                    case "nonFinite": item.FindPropertyRelative("amount").floatValue = float.NaN; break;
                    case "invalidContentType":
                        item.managedReferenceValue = new ContentUnlockEffect();
                        item.FindPropertyRelative("contentId").stringValue = "cards.test";
                        item.FindPropertyRelative("contentType").intValue = 999;
                        break;
                    case "invalidAbilityId":
                        item.managedReferenceValue = new AbilityUnlockEffect();
                        item.FindPropertyRelative("abilityId").stringValue = "bad/id";
                        break;
                }
                data.ApplyModifiedPropertiesWithoutUndo();
                Assert.Throws<InvalidOperationException>(() => new SkillCatalog(new[] { tree }, new PriceCurve(100, 1.5f, 10)));
            }
            finally { UnityEngine.Object.DestroyImmediate(tree); }
        }

        private static SerializedProperty Effects(SerializedObject data) =>
            data.FindProperty("skills.Array.data[0].levels.Array.data[0].effects");

        private static SkillTreeDefinition Load(string id) =>
            AssetDatabase.LoadAssetAtPath<SkillTreeDefinition>($"Assets/Config/Skills/{id}.asset");

        private static string Describe(SkillEffectDefinition effect) => effect switch
        {
            StatModifierEffect s => $"stat:{s.Stat}:{s.TargetId}:{s.Amount.ToString(CultureInfo.InvariantCulture)}",
            GameplayModifierEffect g => $"rule:{g.TargetId}:{g.Amount.ToString(CultureInfo.InvariantCulture)}",
            ContentUnlockEffect c => $"content:{c.ContentType}:{c.TargetId}",
            AbilityUnlockEffect a => $"ability:{a.TargetId}",
            _ => throw new InvalidOperationException("Unexpected effect type.")
        };
    }
}
