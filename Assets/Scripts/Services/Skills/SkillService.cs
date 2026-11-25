using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Game.Services.Config;
using Game.Services.Wallet;

namespace Game.Services.Skills
{
    public sealed class SkillService : ISkillService, IInitializableService
    {
        private readonly IConfigService config;
        private readonly IPurchaseWallet wallet;
        private SkillCatalog catalog;

        public SkillService(IConfigService config, IPurchaseWallet wallet)
        {
            this.config = config;
            this.wallet = wallet;
        }

        public IReadOnlyList<SkillTreeDefinition> Trees => config.SkillTrees;

        public Task InitializeAsync(CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            catalog = null;
            catalog = new SkillCatalog(Trees, config.Economy.Upgrade);
            return Task.CompletedTask;
        }

        public SkillOffer Inspect(SkillContext context, string skillId)
        {
            var skill = catalog?.Find(context, skillId);
            if (skill == null) return new SkillOffer(0, 0, SkillAvailability.Invalid, "Unknown owner, tree or skill.");
            int current = Current(context, skill);
            if (current == skill.Levels.Count) return new SkillOffer(current, 0, SkillAvailability.Mastered, "Maximum level");
            var next = skill.Levels[current];
            int price = SkillPricing.Price(next, config.Economy.Upgrade, current);
            foreach (var req in skill.Requirements.Concat(next.Requirements))
            {
                var required = catalog.Find(context, req.SkillId);
                if (Current(context, required) < req.RequiredLevel)
                    return new SkillOffer(current, price, SkillAvailability.Locked, $"Requires {required.Title} Lv {req.RequiredLevel}");
            }
            return price > wallet.Cash
                ? new SkillOffer(current, price, SkillAvailability.InsufficientCash, "Insufficient Cash")
                : new SkillOffer(current, price, SkillAvailability.Available);
        }

        private int Current(SkillContext context, SkillDefinition skill) =>
            Math.Min(wallet.Purchased(context.Product(skill.Id)), skill.Levels.Count);

        // The caller submits the displayed target. A replay cannot purchase the following level.
        public string Purchase(SkillContext context, string skillId, int targetLevel)
        {
            var offer = Inspect(context, skillId);
            if (!offer.Available) return offer.Reason;
            if (targetLevel != offer.Current + 1) return "Level has changed.";
            try
            {
                return wallet.TryPurchase(context.Product(skillId), targetLevel, offer.Price) ? null : "Purchase unavailable.";
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogWarning($"Skill purchase could not be saved: {exception.Message}");
                return "Could not save purchase. Try again.";
            }
        }

        public IEnumerable<(string skillId, int level, SkillEffectDefinition effect)> GrantedEffects(SkillContext context)
        {
            if (catalog == null) yield break;
            var tree = Trees.FirstOrDefault(t => t.Id == context.TreeId);
            if (tree == null || !SkillCatalog.ValidId(context.OwnerId)) yield break;
            foreach (var skill in tree.Skills)
                for (int i = 0; i < Current(context, skill); i++)
                    foreach (var effect in skill.Levels[i].Effects)
                        yield return (skill.Id, i + 1, effect);
        }
    }
}
