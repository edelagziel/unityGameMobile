using System;
using System.Collections.Generic;
using System.Linq;
using Game.Services.Skills;
using UnityEngine;

namespace Game.ScreenNavigation
{
    public sealed class SkillMapLayout
    {
        public const float NodeRadius = 90;
        public const float CoreRadius = 65;
        public const float FootprintRadius = 170;
        private const float Border = 96;
        private const float FullTurn = 2 * Mathf.PI;
        private readonly Dictionary<string, Vector2> positions = new(StringComparer.Ordinal);
        private readonly Dictionary<string, int> layers = new(StringComparer.Ordinal);
        public IReadOnlyList<SkillDependency> Connections { get; }
        public IReadOnlyList<SkillMapEdge> Edges { get; }
        public Vector2 Size { get; }
        public Vector2 Position(string id) => positions[id];
        public int Layer(string id) => layers[id];

        private sealed class Group
        {
            public readonly List<string> Members = new();
        }

        private sealed class Branch
        {
            public SkillDefinition Skill;
            public SkillDependency ParentEdge;
            public readonly List<Branch> Children = new();
            public int Depth;
            public int Weight;
            public float Angle;
            public float Sector;
        }

        public SkillMapLayout(SkillTreeDefinition tree)
        {
            var skills = tree.Skills.OrderBy(s => s.Id, StringComparer.Ordinal).ToArray();
            Connections = skills.SelectMany(SkillDependency.Incoming).ToArray();
            var order = BuildHierarchy(skills, Connections);
            var roots = order.Where(b => b.ParentEdge == null).ToList();
            for (int i = order.Count - 1; i >= 0; i--)
            {
                var branch = order[i];
                branch.Children.Sort((a, b) =>
                {
                    int category = string.CompareOrdinal(a.Skill.Category, b.Skill.Category);
                    return category != 0 ? category : string.CompareOrdinal(a.Skill.Id, b.Skill.Id);
                });
                branch.Weight = Math.Max(1, branch.Children.Sum(c => c.Weight));
            }

            roots.Sort((a, b) => string.CompareOrdinal(a.Skill.Id, b.Skill.Id));
            for (int i = 0; i < roots.Count; i++)
            {
                roots[i].Angle = FullTurn * i / roots.Count;
                // A single root still fans outward on its own side of Core.
                roots[i].Sector = Mathf.Min(FullTurn / roots.Count, 2 * Mathf.PI / 3);
            }
            var byId = order.ToDictionary(b => b.Skill.Id);
            var incoming = Connections.ToLookup(e => e.Target);
            foreach (var branch in order.OrderBy(b => b.Depth))
            {
                float start = branch.Angle - branch.Sector / 2;
                foreach (var child in branch.Children)
                {
                    child.Sector = branch.Sector * child.Weight / branch.Weight;
                    child.Angle = start + child.Sector / 2;
                    start += child.Sector;
                    // Angle a shared skill toward the other inward prerequisites. Keep the
                    // child's entire subtree inside the sector assigned by its primary parent.
                    float turn = 0;
                    int alternatives = 0;
                    foreach (var edge in incoming[child.Skill.Id])
                    {
                        var other = byId[edge.Source];
                        if (edge == child.ParentEdge || other.Depth >= child.Depth) continue;
                        turn += Mathf.DeltaAngle(child.Angle * Mathf.Rad2Deg, other.Angle * Mathf.Rad2Deg);
                        alternatives++;
                    }
                    if (alternatives > 0)
                    {
                        float desired = turn / alternatives;
                        if (Mathf.Abs(desired) < 5) desired = 25; // Separate collinear ancestor links.
                        float limit = child.Sector * .33f;
                        float shift = Mathf.Clamp(desired * Mathf.Deg2Rad, -limit, limit);
                        child.Angle += shift;
                        child.Sector -= 2 * Mathf.Abs(shift);
                    }
                }
            }

            var clearance = 2 * FootprintRadius + Mathf.Max(0, tree.NodeSpacing);
            float step = Mathf.Max(tree.RingSpacing, clearance);
            var angles = new Dictionary<int, float[]>();
            foreach (var ring in order.GroupBy(b => b.Depth))
            {
                var sorted = ring.Select(b => Mathf.Repeat(b.Angle, FullTurn)).OrderBy(a => a).ToArray();
                angles.Add(ring.Key, sorted);
                if (sorted.Length < 2) continue;
                float gap = FullTurn + sorted[0] - sorted[sorted.Length - 1];
                for (int i = 1; i < sorted.Length; i++) gap = Mathf.Min(gap, sorted[i] - sorted[i - 1]);
                // A single pitch for all rings guarantees clearance and consistent radial increments.
                step = Mathf.Max(step, clearance / (2 * ring.Key * Mathf.Sin(gap / 2)));
            }
            var extent = Vector2.one * CoreRadius;
            foreach (var branch in order)
            {
                var point = branch.Depth * step * new Vector2(Mathf.Cos(branch.Angle), Mathf.Sin(branch.Angle));
                positions.Add(branch.Skill.Id, point);
                layers.Add(branch.Skill.Id, branch.Depth);
                extent = Vector2.Max(extent, Abs(point) + Vector2.one * FootprintRadius);
            }

            var router = new SkillMapRouting(positions, layers, angles, step);
            var edges = new List<SkillMapEdge>();
            var primary = new HashSet<SkillDependency>();
            foreach (var branch in order)
            {
                primary.Add(branch.ParentEdge);
                edges.Add(router.Connect(branch.ParentEdge, branch.Skill.Id, true));
            }
            foreach (var dependency in Connections)
                if (!primary.Contains(dependency)) edges.Add(router.Connect(dependency, dependency.Target, false));
            foreach (var edge in edges)
                foreach (var point in edge.Points) extent = Vector2.Max(extent, Abs(point));
            Edges = edges;
            Size = 2 * (extent + Vector2.one * Border);
        }

        private static Vector2 Abs(Vector2 p) => new Vector2(Mathf.Abs(p.x), Mathf.Abs(p.y));

        // Pick one real incoming edge per skill. Each source component gets one Core entry;
        // within a collapsed level-cycle a directed spanning tree reaches all other members.
        private static List<Branch> BuildHierarchy(SkillDefinition[] skills, IReadOnlyList<SkillDependency> edges)
        {
            var branches = skills.ToDictionary(s => s.Id, s => new Branch { Skill = s });
            var incoming = edges.ToLookup(e => e.Target);
            var outgoing = edges.ToLookup(e => e.Source);
            var order = new List<Branch>();
            foreach (var group in FindGroups(skills, edges))
            {
                var members = group.Members.ToHashSet();
                var entry = group.Members.SelectMany(id => incoming[id]).Where(e => !members.Contains(e.Source))
                    .OrderByDescending(e => branches[e.Source].Depth)
                    .ThenByDescending(e => branches[e.Source].Skill.Category == branches[e.Target].Skill.Category)
                    .ThenBy(e => e.Source, StringComparer.Ordinal).ThenBy(e => e.Target, StringComparer.Ordinal)
                    .FirstOrDefault();
                var queue = new Queue<Branch>();
                void Attach(string id, SkillDependency edge)
                {
                    var branch = branches[id];
                    branch.ParentEdge = edge;
                    branch.Depth = edge == null ? 1 : branches[edge.Source].Depth + 1;
                    if (edge != null) branches[edge.Source].Children.Add(branch);
                    queue.Enqueue(branch);
                    order.Add(branch);
                }
                Attach(entry?.Target ?? group.Members[0], entry);
                while (queue.Count > 0)
                {
                    var from = queue.Dequeue();
                    foreach (var edge in outgoing[from.Skill.Id])
                        if (members.Contains(edge.Target) && branches[edge.Target].Depth == 0)
                            Attach(edge.Target, edge);
                }
            }
            return order;
        }

        // Iterative Kosaraju traversal. The second pass yields strongly connected groups
        // in parent-before-child order, even for valid level graphs with reciprocal skill edges.
        private static List<Group> FindGroups(SkillDefinition[] skills, IReadOnlyList<SkillDependency> edges)
        {
            var outgoing = skills.ToDictionary(s => s.Id, s => new List<string>());
            var incoming = skills.ToDictionary(s => s.Id, s => new List<string>());
            foreach (var edge in edges)
            {
                outgoing[edge.Source].Add(edge.Target);
                incoming[edge.Target].Add(edge.Source);
            }
            var seen = new HashSet<string>();
            var finished = new List<string>();
            var stack = new Stack<(string id, bool exit)>();
            foreach (var skill in skills)
            {
                stack.Push((skill.Id, false));
                while (stack.Count > 0)
                {
                    var (id, exit) = stack.Pop();
                    if (exit) { finished.Add(id); continue; }
                    if (!seen.Add(id)) continue;
                    stack.Push((id, true));
                    foreach (string child in outgoing[id]) stack.Push((child, false));
                }
            }
            var groups = new List<Group>();
            var owner = new Dictionary<string, Group>();
            var pending = new Stack<string>();
            for (int i = finished.Count - 1; i >= 0; i--)
            {
                string start = finished[i];
                if (owner.ContainsKey(start)) continue;
                var group = new Group();
                groups.Add(group);
                pending.Push(start);
                while (pending.Count > 0)
                {
                    string id = pending.Pop();
                    if (!owner.TryAdd(id, group)) continue;
                    group.Members.Add(id);
                    foreach (string parent in incoming[id]) pending.Push(parent);
                }
                group.Members.Sort(StringComparer.Ordinal);
            }

            return groups;
        }
    }
}
