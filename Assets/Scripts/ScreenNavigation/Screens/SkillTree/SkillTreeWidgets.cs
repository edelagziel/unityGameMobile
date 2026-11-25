using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ScreenNavigation
{
    internal static class SkillTreeWidgets
    {
        public static readonly Color Slate = new Color32(47, 60, 77, 255);
        public static readonly Color Green = new Color32(34, 108, 83, 255);
        public static readonly Color Gold = new Color32(134, 94, 39, 255);

        public static RectTransform Rect(Transform parent, string name, Vector2 min, Vector2 max)
        {
            var rect = (RectTransform)new GameObject(name, typeof(RectTransform)).transform;
            rect.gameObject.layer = parent.gameObject.layer;
            rect.SetParent(parent, false);
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            return rect;
        }

        public static Image Paint(RectTransform rect, Color color)
        {
            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            return image;
        }

        public static TextMeshProUGUI Label(RectTransform parent, string value, float size)
        {
            var text = Rect(parent, "Label", Vector2.zero, Vector2.one).gameObject.AddComponent<TextMeshProUGUI>();
            text.text = value;
            text.fontSize = size;
            text.enableAutoSizing = true;
            text.fontSizeMin = Mathf.Min(size, 22);
            text.fontSizeMax = size;
            text.alignment = TextAlignmentOptions.Center;
            text.margin = new Vector4(8, 3, 8, 3);
            text.raycastTarget = false;
            return text;
        }

        public static Button Button(RectTransform host, string caption, Color color, Action click, out TextMeshProUGUI label)
        {
            var image = Paint(host, color);
            var button = host.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => click());
            label = Label(host, caption, 34);
            return button;
        }
    }
}
