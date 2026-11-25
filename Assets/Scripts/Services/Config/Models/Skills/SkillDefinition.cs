using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Services.Skills
{
    [Serializable]
    public sealed class SkillDefinition
    {
        [Tooltip("Persistent ID. Keep it stable after release. Letters, digits, dots, dashes and underscores.")]
        [SerializeField] private string id;
        [SerializeField] private string title;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private string category;
        [SerializeField] private SkillRequirement[] requirements = Array.Empty<SkillRequirement>();
        [SerializeField] private SkillLevelDefinition[] levels = Array.Empty<SkillLevelDefinition>();

        public string Id => id;
        public string Title => title;
        public string Description => description;
        public Sprite Icon => icon;
        public string Category => category;
        public IReadOnlyList<SkillRequirement> Requirements => requirements;
        public IReadOnlyList<SkillLevelDefinition> Levels => levels;
    }
}
