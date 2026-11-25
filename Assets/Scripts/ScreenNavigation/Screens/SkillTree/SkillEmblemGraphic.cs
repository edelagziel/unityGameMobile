using UnityEngine;
using UnityEngine.UI;

namespace Game.ScreenNavigation
{
    // Simple category emblems for definitions that have no authored sprite.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class SkillEmblemGraphic : MaskableGraphic
    {
        public string Category { private get; set; }

        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            var rect = rectTransform.rect;
            void Polygon(params Vector2[] points)
            {
                int start = mesh.currentVertCount;
                foreach (var point in points)
                    mesh.AddVert(rect.center + Vector2.Scale(point, rect.size) * .5f, color, Vector2.zero);
                for (int i = 2; i < points.Length; i++) mesh.AddTriangle(start, start + i - 1, start + i);
            }
            switch (Category)
            {
                case "Army": case "Defense":
                    Polygon(new(-.78f,.78f), new(.78f,.78f), new(.65f,-.28f), new(0,-.94f), new(-.65f,-.28f));
                    break;
                case "Crafting":
                    Polygon(new(-.17f,.44f), new(.17f,.44f), new(.17f,-.95f), new(-.17f,-.95f));
                    Polygon(new(-.82f,.85f), new(.62f,.85f), new(.92f,.28f), new(-.82f,.28f));
                    break;
                case "Cards":
                    Polygon(new(-.95f,.6f), new(-.51f,.78f), new(-.51f,-.75f), new(-.67f,-.78f));
                    Polygon(new(-.38f,.86f), new(.77f,.86f), new(.77f,-.78f), new(-.38f,-.78f));
                    break;
                case "Economy":
                    Polygon(new(-.48f,.85f), new(.48f,.85f), new(.86f,.43f), new(.86f,-.43f),
                        new(.48f,-.85f), new(-.48f,-.85f), new(-.86f,-.43f), new(-.86f,.43f));
                    break;
                default:
                    Polygon(new(0,.94f), new(.64f,0), new(0,-.94f), new(-.64f,0));
                    break;
            }
        }
    }
}
