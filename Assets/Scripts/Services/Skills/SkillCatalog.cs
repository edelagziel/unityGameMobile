using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Game.Services.Config;

namespace Game.Services.Skills
{
    public sealed class SkillCatalog
    {
        private readonly Dictionary<string, Dictionary<string, SkillDefinition>> trees = new();
        internal static bool ValidId(string id) => id != null && Regex.IsMatch(id, @"\A[a-zA-Z0-9_.-]+\z");

        public SkillCatalog(IReadOnlyList<SkillTreeDefinition> definitions, PriceCurve curve)
        {
            if (definitions == null || definitions.Count == 0) throw Invalid("Assign at least one tree in GameConfig.");
            foreach (var tree in definitions)
            {
                if (!tree || !ValidId(tree.Id) || trees.ContainsKey(tree.Id))
                    throw Invalid($"Missing tree or invalid/duplicate tree ID '{tree?.Id}'.");
                if (tree.Skills == null || tree.Skills.Count == 0 || string.IsNullOrWhiteSpace(tree.Title))
                    throw Invalid($"{tree.Id}: supply a title and skills.");
                var skills = new Dictionary<string, SkillDefinition>();
                foreach (var skill in tree.Skills)
                {
                    if (skill == null || !ValidId(skill.Id) || skills.ContainsKey(skill.Id))
                        throw Invalid($"{tree.Id}: invalid or duplicate skill '{skill?.Id}'.");
                    if (string.IsNullOrWhiteSpace(skill.Title) || skill.Levels == null || skill.Levels.Count == 0)
                        throw Invalid($"{tree.Id}/{skill.Id}: supply a title and at least one level.");
                    skills.Add(skill.Id, skill);
                }
                ValidateTree(tree.Id, skills, curve);
                trees.Add(tree.Id, skills);
            }
        }

        public SkillDefinition Find(SkillContext context, string id) =>
            ValidId(context.OwnerId) && ValidId(context.TreeId) && ValidId(id)
            && trees.TryGetValue(context.TreeId, out var skills) && skills.TryGetValue(id, out var skill) ? skill : null;

        private static void ValidateTree(string tree, Dictionary<string, SkillDefinition> skills, PriceCurve curve)
        {
            var visiting = new HashSet<(string, int)>();
            var finished = new HashSet<(string, int)>();
            foreach (var skill in skills.Values)
                Visit(skill, skill.Levels.Count);

            void Visit(SkillDefinition skill, int number)
            {
                var key = (skill.Id, number);
                if (finished.Contains(key)) return;
                string path = $"{tree}/{skill.Id} level {number}";
                if (!visiting.Add(key)) throw Invalid($"{path}: dependency cycle makes this level unreachable.");
                if (number > 1) Visit(skill, number - 1);
                var level = skill.Levels[number - 1];
                if (level == null) throw Invalid($"{path}: missing level.");
                if (level.UseEconomyCurve ?
                    curve.BaseCost < 1 || !(curve.Growth >= 1) || float.IsInfinity(curve.Growth) || number > curve.MaxLevel :
                    level.FixedCash < 1)
                    throw Invalid($"{path}: invalid price or level exceeds the economy curve.");
                if (level.Effects == null || level.Effects.Count == 0 || level.Effects.Any(e => e == null || !ValidId(e.TargetId) || !e.HasValidParameters))
                    throw Invalid($"{path}: supply valid effects and stable target IDs.");
                if (skill.Requirements == null || level.Requirements == null) throw Invalid($"{path}: missing requirements list.");
                foreach (var req in skill.Requirements.Concat(level.Requirements))
                {
                    if (req == null || !ValidId(req.SkillId) || !skills.TryGetValue(req.SkillId, out var other)
                        || req.SkillId == skill.Id || req.RequiredLevel < 1 || req.RequiredLevel > other.Levels.Count)
                        throw Invalid($"{path}: invalid prerequisite '{req?.SkillId}' level {req?.RequiredLevel}.");
                    Visit(other, req.RequiredLevel);
                }
                visiting.Remove(key);
                finished.Add(key);
            }
        }

        private static InvalidOperationException Invalid(string message) => new InvalidOperationException("Skill catalog: " + message);
    }
}
