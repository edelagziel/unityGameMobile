using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ScreenNavigation
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class SkillArrowGraphic : MaskableGraphic
    {
        private readonly List<Vector2> points = new();
        private bool primary;
        public Vector2 StartPoint => points[0];
        public Vector2 EndPoint => points[points.Count - 1];

        public void SetRoute(SkillMapEdge edge)
        {
            primary = edge.Primary;
            var min = edge.Points[0];
            var max = min;
            foreach (var point in edge.Points)
            {
                min = Vector2.Min(min, point);
                max = Vector2.Max(max, point);
            }
            // A tight local rect lets RectMask2D cull the entire stroke correctly.
            var center = (min + max) / 2;
            rectTransform.anchoredPosition = center;
            rectTransform.sizeDelta = max - min + Vector2.one * 22;
            points.Clear();
            foreach (var point in edge.Points) points.Add(point - center);
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            float phase = 0;
            for (int i = 1; i < points.Count; i++)
            {
                Vector2 a = points[i - 1], b = points[i];
                var direction = (b - a).normalized;
                float length = Vector2.Distance(a, b);
                if (length < .001f) continue;
                if (i == points.Count - 1)
                {
                    length -= Mathf.Min(16, length / 2);
                    Vector2 neck = a + direction * length;
                    Vector2 side = Vector2.Perpendicular(direction) * (primary ? 10 : 7);
                    int tip = mesh.currentVertCount;
                    mesh.AddVert(neck + side, color, Vector2.zero);
                    mesh.AddVert(b, color, Vector2.zero);
                    mesh.AddVert(neck - side, color, Vector2.zero);
                    mesh.AddTriangle(tip, tip + 1, tip + 2);
                }
                if (primary) Stroke(mesh, a, a + direction * length, direction, 3.5f);
                else
                {
                    const float period = 28, dash = 17;
                    for (float start = -phase; start < length; start += period)
                    {
                        float end = Mathf.Min(start + dash, length);
                        if (end > 0) Stroke(mesh, a + direction * Mathf.Max(0, start), a + direction * end, direction, 2);
                    }
                    phase = Mathf.Repeat(phase + length, period);
                }
            }
        }

        private void Stroke(VertexHelper mesh, Vector2 a, Vector2 b, Vector2 direction, float halfWidth)
        {
            var side = Vector2.Perpendicular(direction) * halfWidth;
            int start = mesh.currentVertCount;
            mesh.AddVert(a - side, color, Vector2.zero);
            mesh.AddVert(a + side, color, Vector2.zero);
            mesh.AddVert(b + side, color, Vector2.zero);
            mesh.AddVert(b - side, color, Vector2.zero);
            mesh.AddTriangle(start, start + 1, start + 2);
            mesh.AddTriangle(start, start + 2, start + 3);
        }
    }
}
