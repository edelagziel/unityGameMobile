using System;
using System.Collections.Generic;
using System.Linq;
using Game.Services.Skills;
using TMPro;
using UnityEngine;
using static Game.ScreenNavigation.SkillTreeWidgets;

namespace Game.ScreenNavigation
{
    internal sealed class SkillDetailsPanel
    {
        private readonly TextMeshProUGUI details;
        private readonly TextMeshProUGUI status;
        private readonly TextMeshProUGUI caption;
        private readonly SkillPurchaseButton purchase;
        private SkillDefinition selected;
        private SkillContext context;
        private int current = -1;
        private int price = -1;
        private IReadOnlyDictionary<string, SkillOffer> previousOffers;

        public SkillDetailsPanel(RectTransform panel, Action<SkillContext, string, int> buy)
        {
            details = Label(Rect(panel, "Details", new Vector2(.03f, .15f), new Vector2(.97f, .38f)), "", 34);
            status = Label(Rect(panel, "Status", new Vector2(.03f, .095f), new Vector2(.97f, .15f)), "", 27);
            var host = Rect(panel, "Purchase", new Vector2(.12f, .015f), new Vector2(.88f, .093f));
            var background = Paint(host, Green);
            purchase = host.gameObject.AddComponent<SkillPurchaseButton>();
            purchase.targetGraphic = background;
            purchase.onClick.AddListener(() => buy(context, selected.Id, current + 1));
            caption = Label(host, "", 34);
        }

        public void Show(SkillContext owner, SkillDefinition skill, IReadOnlyDictionary<string, SkillOffer> offers,
            Func<string, SkillDefinition> findSkill, string feedback)
        {
            var offer = offers[skill.Id];
            if (owner.OwnerId != context.OwnerId || selected != skill || current != offer.Current)
                purchase.InvalidatePress();
            var next = offer.Current < skill.Levels.Count ? skill.Levels[offer.Current] : null;
            if (selected != skill || current != offer.Current || RequirementsChanged(skill, next, offers))
            {
                string text = $"{skill.Category}   /   {skill.Title}   {offer.Current}/{skill.Levels.Count}\n{skill.Description}";
                if (next != null)
                {
                    text += $"\nNext: {next.Description}\n" + string.Join("   •   ", next.Effects.Select(Describe));
                    var requirements = skill.Requirements.Concat(next.Requirements).Select(r =>
                        $"{findSkill(r.SkillId).Title} {offers[r.SkillId].Current}/{r.RequiredLevel}");
                    if (skill.Requirements.Count + next.Requirements.Count > 0)
                        text += "\nRequires: " + string.Join(", ", requirements);
                }
                else
                    text += "\nGranted: " + string.Join("   •   ", skill.Levels.SelectMany(l => l.Effects).Select(Describe));
                details.text = text;
            }
            selected = skill;
            context = owner;
            current = offer.Current;
            previousOffers = offers;
            status.text = feedback ?? offer.Reason ?? "";
            if (price != offer.Price)
            {
                price = offer.Price;
                caption.text = price > 0 ? $"Upgrade   ·   {price} Cash" : "Maximum level";
            }
            purchase.interactable = offer.Available;
        }

        private bool RequirementsChanged(SkillDefinition skill, SkillLevelDefinition next, IReadOnlyDictionary<string, SkillOffer> offers)
        {
            if (next == null || ReferenceEquals(previousOffers, offers)) return false;
            foreach (var r in skill.Requirements)
                if (previousOffers[r.SkillId].Current != offers[r.SkillId].Current) return true;
            foreach (var r in next.Requirements)
                if (previousOffers[r.SkillId].Current != offers[r.SkillId].Current) return true;
            return false;
        }

        private static string Describe(SkillEffectDefinition effect) => effect is NumericModifierEffect modifier
            ? $"{modifier.TargetId}  {modifier.Amount:+0.##;-0.##;0}"
            : "Unlock " + effect.TargetId;
    }
}
