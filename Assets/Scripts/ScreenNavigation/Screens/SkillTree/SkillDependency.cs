using System;
using System.Collections.Generic;
using Game.Services.Skills;

namespace Game.ScreenNavigation
{
    // One visual edge per ordered pair. Retain the target level of each gate for coloring.
    public sealed class SkillDependency
    {
        private int baseRequirement;
        private readonly Dictionary<int, int> levelRequirements = new();
        public string Source { get; }
        public string Target { get; }
        public int HighestRequirement { get; private set; }

        private SkillDependency(string source, string target)
        {
            Source = source;
            Target = target;
        }

        // Zero means the edge does not gate this particular target level.
        public int RequiredFor(int targetLevel)
        {
            levelRequirements.TryGetValue(targetLevel, out int minimum);
            return Math.Max(baseRequirement, minimum);
        }

        internal static IEnumerable<SkillDependency> Incoming(SkillDefinition skill)
        {
            var edges = new SortedDictionary<string, SkillDependency>(StringComparer.Ordinal);
            void Add(SkillRequirement requirement, int targetLevel)
            {
                if (!edges.TryGetValue(requirement.SkillId, out var edge))
                    edges.Add(requirement.SkillId, edge = new SkillDependency(requirement.SkillId, skill.Id));
                edge.HighestRequirement = Math.Max(edge.HighestRequirement, requirement.RequiredLevel);
                if (targetLevel == 0)
                    edge.baseRequirement = Math.Max(edge.baseRequirement, requirement.RequiredLevel);
                else
                {
                    edge.levelRequirements.TryGetValue(targetLevel, out int previous);
                    edge.levelRequirements[targetLevel] = Math.Max(previous, requirement.RequiredLevel);
                }
            }
            foreach (var requirement in skill.Requirements) Add(requirement, 0);
            for (int i = 0; i < skill.Levels.Count; i++)
                foreach (var requirement in skill.Levels[i].Requirements) Add(requirement, i + 1);
            return edges.Values;
        }
    }
}
