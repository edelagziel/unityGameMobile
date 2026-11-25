using UnityEngine;
using UnityEngine.UI;

namespace Game.ScreenNavigation
{
    // A resolution-independent disc. The enclosing square remains a generous touch target.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class SkillCircleGraphic : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            Rect rect = rectTransform.rect;
            float radius = Mathf.Min(rect.width, rect.height) / 2;
            mesh.AddVert(rect.center, color, Vector2.zero);
            const int sides = 48;
            for (int i = 0; i < sides; i++)
            {
                float angle = i * (2 * Mathf.PI / sides);
                mesh.AddVert(rect.center + radius * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), color, Vector2.zero);
            }
            for (int i = 0; i < sides; i++) mesh.AddTriangle(0, 1 + (i + 1) % sides, 1 + i);
        }
    }
}
