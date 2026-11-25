using System;
using System.Linq;
using Game.Services.Skills;
using UnityEditor;
using UnityEngine;

namespace Game.EditorTools
{
    [CustomPropertyDrawer(typeof(SkillEffectListAttribute))]
    public sealed class SkillEffectListDrawer : PropertyDrawer
    {
        private static readonly Type[] Types = TypeCache.GetTypesDerivedFrom<SkillEffectDefinition>()
            .Where(t => !t.IsAbstract && !t.ContainsGenericParameters && t.IsSerializable &&
                t.GetConstructor(Type.EmptyTypes) != null)
            .OrderBy(t => t.Name, StringComparer.Ordinal).ToArray();

        private static readonly string[] Choices = new[] { "Choose type" }.Concat(Types.Select(t =>
            ObjectNames.NicifyVariableName(t.Name))).ToArray();

        private static float Row => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = Row;
            if (!property.isExpanded || property.serializedObject.isEditingMultipleObjects) return height;
            for (int i = 0; i < property.arraySize; i++)
            {
                height += Row;
                var element = property.GetArrayElementAtIndex(i);
                height += DrawFields(element, default, false);
            }
            return height + Row;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            try
            {
                var row = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                if (property.serializedObject.isEditingMultipleObjects)
                {
                    EditorGUI.LabelField(row, label.text, "Select one tree to edit effects");
                    return;
                }

                property.isExpanded = EditorGUI.Foldout(row, property.isExpanded, $"{label.text} ({property.arraySize})", true);
                if (!property.isExpanded) return;
                row = EditorGUI.IndentedRect(row);
                for (int i = 0; i < property.arraySize; i++)
                {
                    row.y += Row;
                    var element = property.GetArrayElementAtIndex(i);
                    var selector = row;
                    selector.width -= 28;
                    int current = Array.IndexOf(Types, element.managedReferenceValue?.GetType()) + 1;
                    int selected = EditorGUI.Popup(selector, $"Effect {i + 1}", current, Choices);
                    if (selected > 0 && selected != current)
                    {
                        element.managedReferenceValue = Activator.CreateInstance(Types[selected - 1]);
                        return;
                    }
                    if (GUI.Button(new Rect(selector.xMax + 2, row.y, 26, row.height), "−"))
                    {
                        element.managedReferenceValue = null;
                        property.DeleteArrayElementAtIndex(i);
                        return;
                    }

                    var children = row;
                    children.y += Row;
                    row.y += DrawFields(element, children, true);
                }
                row.y += Row;
                if (GUI.Button(row, "Add effect"))
                {
                    int index = property.arraySize++;
                    property.GetArrayElementAtIndex(index).managedReferenceValue = null;
                }
            }
            finally { EditorGUI.EndProperty(); }
        }

        // Iterate direct serialized fields, including inherited modifier amounts.
        private static float DrawFields(SerializedProperty element, Rect row, bool draw)
        {
            if (element.managedReferenceValue == null) return 0;
            var child = element.Copy();
            if (!child.NextVisible(true)) return 0;
            float height = 0;
            while (child.depth == element.depth + 1)
            {
                row.height = EditorGUI.GetPropertyHeight(child, true);
                if (draw) EditorGUI.PropertyField(row, child, true);
                float step = row.height + EditorGUIUtility.standardVerticalSpacing;
                height += step;
                row.y += step;
                if (!child.NextVisible(false)) break;
            }
            return height;
        }
    }
}
