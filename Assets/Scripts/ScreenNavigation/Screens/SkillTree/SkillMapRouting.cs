using System.Collections.Generic;
using UnityEngine;

namespace Game.ScreenNavigation
{
    // All points are in the common map coordinate system, including the clipped endpoints.
    internal sealed class SkillMapRouting
    {
        private readonly Dictionary<string, Vector2> positions;
        private readonly Dictionary<string, int> layers;
        private readonly Dictionary<int, float[]> ringAngles;
        private readonly float step;

        public SkillMapRouting(Dictionary<string, Vector2> positions, Dictionary<string, int> layers,
            Dictionary<int, float[]> ringAngles, float step)
        {
            this.positions = positions;
            this.layers = layers;
            this.ringAngles = ringAngles;
            this.step = step;
        }

        public SkillMapEdge Connect(SkillDependency dependency, string target, bool primary)
        {
            string source = dependency?.Source;
            var start = source == null ? Vector2.zero : positions[source];
            var end = positions[target];
            float startRadius = source == null ? SkillMapLayout.CoreRadius : SkillMapLayout.NodeRadius;
            var axis = (end - start).normalized;
            // Alternate prerequisites have a separate attachment lane, including reciprocal pairs.
            float lane = primary ? 0 : 14;
            var offset = lane * Vector2.Perpendicular(axis);
            var points = new List<Vector2>
            {
                start + offset + axis * Mathf.Sqrt(startRadius * startRadius - lane * lane),
                end + offset - axis * Mathf.Sqrt(SkillMapLayout.NodeRadius * SkillMapLayout.NodeRadius - lane * lane)
            };
            if (source != null && !Clear(points[0], points[1], source, target))
            {
                bool backwards = layers[source] > layers[target];
                points = backwards ? BetweenRings(target, source) : BetweenRings(source, target);
                if (backwards) points.Reverse();
                points = Shorten(points, source, target);
                int last = points.Count - 1;
                var firstDirection = (points[1] - start).normalized;
                var lastDirection = (end - points[last - 1]).normalized;
                // Keep the ends of a dashed detour visibly separate from a radial primary stroke.
                if (!primary)
                {
                    points[1] += Vector2.Perpendicular(firstDirection) * lane;
                    if (last - 1 != 1) points[last - 1] += Vector2.Perpendicular(lastDirection) * lane;
                }
                points[0] = start + firstDirection * Mathf.Sqrt(startRadius * startRadius - lane * lane)
                    + Vector2.Perpendicular(firstDirection) * lane;
                points[last] = end - lastDirection * Mathf.Sqrt(SkillMapLayout.NodeRadius * SkillMapLayout.NodeRadius - lane * lane)
                    + Vector2.Perpendicular(lastDirection) * lane;
            }
            return new SkillMapEdge(dependency, target, primary, points);
        }

        // The ring corridors provide a safe path. Skip turns whenever a shorter chord
        // also clears the obstacles, so a cross-branch link remains easy to follow.
        private List<Vector2> Shorten(List<Vector2> path, string source, string target)
        {
            int last = path.Count - 1;
            var result = new List<Vector2> { path[0] };
            for (int from = 0; from < last;)
            {
                int next = last;
                while (next > from + 1 && !Clear(path[from], path[next],
                    from == 0 ? source : null, next == last ? target : null)) next--;
                result.Add(path[next]);
                from = next;
            }
            return result;
        }

        private bool Clear(Vector2 a, Vector2 b, string source, string target)
        {
            if (NearSegment(a, b, Vector2.zero, SkillMapLayout.CoreRadius + 12)) return false;
            foreach (var node in positions)
                if (node.Key != source && node.Key != target &&
                    NearSegment(a, b, node.Value, SkillMapLayout.FootprintRadius + 5)) return false;
            return true;
        }

        private static bool NearSegment(Vector2 a, Vector2 b, Vector2 center, float clearance)
        {
            var delta = b - a;
            float t = Mathf.Clamp01(Vector2.Dot(center - a, delta) / delta.sqrMagnitude);
            return (a + t * delta - center).sqrMagnitude < clearance * clearance;
        }

        // Travel between occupied rings. Cross any intermediate ring through a gap between nodes.
        private List<Vector2> BetweenRings(string inner, string outer)
        {
            int first = layers[inner];
            int last = layers[outer];
            float angle = Bearing(positions[inner]);
            float destination = Bearing(positions[outer]);
            var points = new List<Vector2> { positions[inner] };
            for (int ring = first; ring < Mathf.Max(first + 1, last); ring++)
            {
                float radius = (ring + .5f) * step;
                float next = ring + 1 < last ? Passage(ring + 1, destination) : destination;
                points.Add(Polar(radius, angle));
                float turn = Mathf.DeltaAngle(angle * Mathf.Rad2Deg, next * Mathf.Rad2Deg) * Mathf.Deg2Rad;
                // Limit the inward sag of each chord so the path stays in its empty corridor.
                float maxTurn = Mathf.Min(.25f, 2 * Mathf.Acos(Mathf.Clamp(1 - 4 / radius, -1, 1)));
                int segments = Mathf.CeilToInt(Mathf.Abs(turn) / Mathf.Max(.001f, maxTurn));
                for (int i = 1; i <= segments; i++) points.Add(Polar(radius, angle + turn * i / segments));
                angle = next;
            }
            points.Add(positions[outer]);
            return points;
        }

        private float Passage(int layer, float desired)
        {
            var angles = ringAngles[layer];
            float radius = layer * step;
            var crossing = Polar(radius, desired);
            foreach (float angle in angles)
                if (NearSegment(Polar(radius - step / 2, desired), Polar(radius + step / 2, desired),
                    Polar(radius, angle), SkillMapLayout.FootprintRadius + 5))
                {
                    float best = 0;
                    float distance = float.PositiveInfinity;
                    for (int i = 0; i < angles.Length; i++)
                    {
                        float right = i + 1 < angles.Length ? angles[i + 1] : angles[0] + 2 * Mathf.PI;
                        float mid = (angles[i] + right) / 2;
                        float cost = (Polar(radius, mid) - crossing).sqrMagnitude;
                        if (cost < distance) { best = mid; distance = cost; }
                    }
                    return best;
                }
            return desired;
        }

        private static float Bearing(Vector2 v) => Mathf.Atan2(v.y, v.x);
        private static Vector2 Polar(float radius, float angle) =>
            radius * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
}
