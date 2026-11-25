using System;
using Game.Services.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Game.ScreenNavigation.SkillTreeWidgets;

namespace Game.ScreenNavigation
{
    internal sealed class SkillNodeView
    {
        public SkillDefinition Skill { get; }
        public RectTransform RectTransform { get; }
        private readonly TextMeshProUGUI levelLabel;
        private readonly Outline outline;
        private readonly SkillCircleGraphic face;
        private int level = -1;
        private SkillAvailability state;

        public SkillNodeView(RectTransform parent, SkillDefinition skill, Vector2 position, Action<string> select)
        {
            Skill = skill;
            var center = Vector2.one * .5f;
            RectTransform = Rect(parent, skill.Title, center, center);
            RectTransform.anchoredPosition = position;
            RectTransform.sizeDelta = Vector2.one * (2 * SkillMapLayout.NodeRadius);
            face = RectTransform.gameObject.AddComponent<SkillCircleGraphic>();
            var button = RectTransform.gameObject.AddComponent<Button>();
            button.targetGraphic = face;
            button.onClick.AddListener(() => select(skill.Id));
            outline = RectTransform.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color32(255, 242, 184, 255);
            outline.effectDistance = new Vector2(5, 5);
            outline.enabled = false;

            var icon = Rect(RectTransform, "Icon", new Vector2(.30f, .30f), new Vector2(.70f, .70f));
            if (skill.Icon)
            {
                var image = Paint(icon, Color.white);
                image.sprite = skill.Icon;
                image.preserveAspect = true;
                image.raycastTarget = false;
            }
            else
            {
                var emblem = icon.gameObject.AddComponent<SkillEmblemGraphic>();
                emblem.Category = skill.Category;
                emblem.raycastTarget = false;
            }
            Label(Rect(RectTransform, "Branch", new Vector2(.11f, .70f), new Vector2(.89f, .93f)), skill.Category, 25);
            levelLabel = Label(Rect(RectTransform, "Level", new Vector2(.14f,.04f), new Vector2(.86f,.32f)), "", 29);
            var caption = Rect(RectTransform, "Name", new Vector2(.5f, 0), new Vector2(.5f, 0));
            caption.sizeDelta = new Vector2(216, 40);
            caption.anchoredPosition = Vector2.down * 20;
            Label(caption, skill.Title, 28);
        }

        public void Select(bool selected) => outline.enabled = selected;

        public void ShowOffer(SkillOffer offer)
        {
            if (level == offer.Current && state == offer.State) return;
            if (level != offer.Current) levelLabel.text = $"{offer.Current}/{Skill.Levels.Count}";
            level = offer.Current;
            state = offer.State;
            face.color = state == SkillAvailability.Mastered ? Gold :
                level > 0 ? new Color32(49, 105, 157, 255) :
                state == SkillAvailability.Available ? Green :
                state == SkillAvailability.InsufficientCash ? new Color32(111, 77, 66, 255) : Slate;
        }
    }
}
