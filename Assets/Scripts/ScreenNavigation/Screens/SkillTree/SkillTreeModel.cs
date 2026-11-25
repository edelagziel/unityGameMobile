using System.Collections.Generic;
using System.Linq;
using Game.Services.Skills;

namespace Game.ScreenNavigation
{
    public sealed class SkillTreeModel : ISkillTreeModel
    {
        private readonly ISkillService skills;
        public SkillContext Context { get; private set; }
        public SkillTreeDefinition Tree { get; private set; }
        public string Selection { get; private set; }

        public static bool IsMaxed(Game.Services.Config.PriceCurve curve, int level) => level >= curve.MaxLevel;

        public static int UpgradeCost(Game.Services.Config.PriceCurve curve, int level) =>
            SkillPricing.CurveCost(curve, level);

        public SkillTreeModel(ISkillService skills)
        {
            this.skills = skills;
            if (skills.Trees.Count > 0) Cycle();
        }

        public bool Open(SkillContext context)
        {
            var tree = skills.Trees.FirstOrDefault(t => t.Id == context.TreeId);
            if (tree == null || !SkillContext.IsValidOwner(context.OwnerId)) return false;
            Tree = tree;
            Context = context;
            Selection = tree.Skills[0].Id;
            return true;
        }

        public void Cycle()
        {
            int index = skills.Trees.ToList().IndexOf(Tree);
            var next = skills.Trees[(index + 1) % skills.Trees.Count];
            Open(new SkillContext(next.Id, next.Id == "global" ? "player" : "type." + next.Id));
        }

        public void Select(string id)
        {
            if (Tree.Skills.Any(s => s.Id == id)) Selection = id;
        }

        public IReadOnlyDictionary<string, SkillOffer> Offers() =>
            Tree.Skills.ToDictionary(s => s.Id, s => skills.Inspect(Context, s.Id));

        public string Purchase(SkillContext context, string id, int targetLevel) =>
            context.Equals(Context) && id == Selection ? skills.Purchase(context, id, targetLevel) : "Selection changed.";
    }
}
