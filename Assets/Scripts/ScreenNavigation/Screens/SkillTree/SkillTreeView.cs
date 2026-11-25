using System;
using System.Collections.Generic;
using Game.Services.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Game.ScreenNavigation.SkillTreeWidgets;

namespace Game.ScreenNavigation
{
    public sealed class SkillTreeView : TabbedScreenView, ISkillTreeView
    {
        private SkillGraphRenderer graph;
        private SkillDetailsPanel details;
        private MapPanZoomController map;
        private TextMeshProUGUI treeLabel;
        private SkillTreeDefinition displayedTree;
        private int displayedRevision;
        private SkillContext context;
        private IReadOnlyDictionary<string, SkillOffer> offers;

        public bool Visible => isActiveAndEnabled;
        public event Action CycleRequested;
        public event Action<string> Selected;
        public event Action<SkillContext, string, int> PurchaseRequested;

        public void Render(SkillContext owner, SkillTreeDefinition tree, string selection,
            IReadOnlyDictionary<string, SkillOffer> currentOffers, int cash, string feedback)
        {
            if (graph == null) BuildPanel();
            if (displayedTree != tree || displayedRevision != tree.DefinitionRevision)
            {
                graph.Build(tree, new SkillMapLayout(tree));
                displayedRevision = tree.DefinitionRevision;
                treeLabel.text = tree.Title + "   >";
                displayedTree = tree;
            }
            context = owner;
            offers = currentOffers;
            graph.ShowOffers(offers);
            ShowSelection(selection, feedback);
        }

        public void ShowSelection(string id, string feedback = null)
        {
            var skill = graph.Select(id);
            details.Show(context, skill, offers, graph.GetSkill, feedback);
        }

        private void BuildPanel()
        {
            var panel = Rect(transform, "Skill Tree", Vector2.zero, Vector2.one);
            Paint(panel, new Color32(21, 29, 41, 255));
            var viewport = Rect(panel, "Graph", new Vector2(.02f, .39f), new Vector2(.98f, .85f));
            Paint(viewport, new Color32(28, 40, 55, 255));
            viewport.gameObject.AddComponent<RectMask2D>();
            map = viewport.gameObject.AddComponent<MapPanZoomController>();
            map.viewport = viewport;

            graph = new SkillGraphRenderer(map, id => { if (!map.BlocksClick) Selected?.Invoke(id); });
            var header = Rect(panel, "Tree", new Vector2(.02f, .86f), new Vector2(.48f, .916f));
            Button(header, "", Slate, () => { if (!map.BlocksClick) CycleRequested?.Invoke(); }, out treeLabel);
            var center = Rect(panel, "Center", new Vector2(.5f, .86f), new Vector2(.71f, .916f));
            Button(center, "Center", Slate, () => { if (!map.BlocksClick) map.Focus(); }, out _);
            details = new SkillDetailsPanel(panel, (owner, id, target) =>
            {
                if (!map.BlocksClick) PurchaseRequested?.Invoke(owner, id, target);
            });
        }
    }
}
