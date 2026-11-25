using System.Collections.Generic;

namespace Game.Services.Skills
{
    public interface ISkillService
    {
        IReadOnlyList<SkillTreeDefinition> Trees { get; }
        SkillOffer Inspect(SkillContext context, string skillId);
        string Purchase(SkillContext context, string skillId, int targetLevel);
        IEnumerable<(string skillId, int level, SkillEffectDefinition effect)> GrantedEffects(SkillContext context);
    }
}
