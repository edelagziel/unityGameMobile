using System;
using System.Collections.Generic;
using Game.Services.Skills;
using UnityEngine;
using static Game.ScreenNavigation.SkillTreeWidgets;

namespace Game.ScreenNavigation
{
    internal sealed class SkillGraphRenderer
    {
        private readonly MapPanZoomController map;
        private readonly Dictionary<string, SkillNodeView> nodes = new();
        private readonly List<(SkillMapEdge edge, SkillArrowGraphic arrow)> connections = new();
        private readonly Action<string> select;
        private SkillNodeView selected;

        public SkillGraphRenderer(MapPanZoomController map, Action<string> select)
        {
            this.map = map;
            this.select = select;
        }

        public void Build(SkillTreeDefinition tree, SkillMapLayout layout)
        {
            if (map.content)
            {
                map.content.gameObject.SetActive(false);
                if (Application.isPlaying) UnityEngine.Object.Destroy(map.content.gameObject);
                else UnityEngine.Object.DestroyImmediate(map.content.gameObject);
            }
            selected = null;
            nodes.Clear();
            connections.Clear();
            var center = Vector2.one * .5f;
            var content = Rect(map.viewport, "Branches", center, center);
            content.sizeDelta = layout.Size;
            var edges = Rect(content, "Connections", Vector2.zero, Vector2.one);
            foreach (var edge in layout.Edges)
            {
                var line = Rect(edges, (edge.Source ?? "Core") + " to " + edge.Target, center, center);
                var arrow = line.gameObject.AddComponent<SkillArrowGraphic>();
                arrow.SetRoute(edge);
                arrow.raycastTarget = false;
                if (edge.Dependency == null) arrow.color = new Color32(158, 182, 209, 255);
                if (!edge.Primary)
                {
                    line.SetAsFirstSibling();
                    line.gameObject.SetActive(false);
                }
                connections.Add((edge, arrow));
            }
            var core = Rect(content, "Core", center, center);
            core.sizeDelta = Vector2.one * (2 * SkillMapLayout.CoreRadius);
            var disc = core.gameObject.AddComponent<SkillCircleGraphic>();
            disc.color = new Color32(61, 60, 72, 255);
            disc.raycastTarget = false;
            Label(core, "Core", 32);
            foreach (var skill in tree.Skills)
                nodes.Add(skill.Id, new SkillNodeView(content, skill, layout.Position(skill.Id), select));
            map.SetMap(content, Vector2.zero, overviewOnFocus: true);
        }

        public void ShowOffers(IReadOnlyDictionary<string, SkillOffer> offers)
        {
            foreach (var entry in offers) nodes[entry.Key].ShowOffer(entry.Value);
            foreach (var (edge, arrow) in connections)
            {
                var gate = edge.Dependency;
                if (gate == null) continue;
                int target = offers[edge.Target].Current;
                bool mastered = target == nodes[edge.Target].Skill.Levels.Count;
                int required = mastered ? gate.HighestRequirement : gate.RequiredFor(target + 1);
                Color color;
                if (required > 0 && offers[edge.Source].Current >= required)
                    color = new Color32(100, 212, 163, 255);
                else if (required > 0)
                    color = new Color32(207, 147, 96, 255);
                else
                    color = new Color32(95, 110, 133, 255);
                if (arrow.color != color) arrow.color = color;
            }
        }

        public SkillDefinition Select(string id)
        {
            var node = nodes[id];
            if (selected != node)
            {
                selected?.Select(false);
                selected = node;
                selected.Select(true);
                foreach (var (edge, arrow) in connections)
                    if (!edge.Primary) arrow.gameObject.SetActive(edge.Target == id);
            }
            return node.Skill;
        }

        public SkillDefinition GetSkill(string id) => nodes[id].Skill;
    }
}
