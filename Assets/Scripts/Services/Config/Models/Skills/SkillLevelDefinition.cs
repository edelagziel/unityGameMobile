using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Services.Skills
{
    [Serializable]
    public sealed class SkillLevelDefinition
    {
        [SerializeField] private string description;
        [Tooltip("Use Economy > Upgrade at this level. Otherwise charge Fixed Cash.")]
        [SerializeField] private bool useEconomyCurve;
        [SerializeField] private int fixedCash;
        [SerializeField] private SkillRequirement[] requirements = Array.Empty<SkillRequirement>();
        [SkillEffectList, SerializeReference] private SkillEffectDefinition[] effects = Array.Empty<SkillEffectDefinition>();

        public string Description => description;
        public bool UseEconomyCurve => useEconomyCurve;
        public int FixedCash => fixedCash;
        public IReadOnlyList<SkillRequirement> Requirements => requirements;
        public IReadOnlyList<SkillEffectDefinition> Effects => effects;
    }
}
