using System.Collections.Generic;
using Game.Services.Skills;

namespace Game.ScreenNavigation
{
    public interface ISkillTreeModel
    {
        SkillContext Context { get; }
        SkillTreeDefinition Tree { get; }
        string Selection { get; }
        bool Open(SkillContext context);
        void Cycle();
        void Select(string id);
        IReadOnlyDictionary<string, SkillOffer> Offers();
        string Purchase(SkillContext context, string id, int targetLevel);
    }
}
