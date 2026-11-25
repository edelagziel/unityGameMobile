using System;
using UnityEngine;

namespace Game.Services.Skills
{
    [Serializable]
    public sealed class AbilityUnlockEffect : SkillEffectDefinition
    {
        [SerializeField] private string abilityId;
        public override string TargetId => abilityId;
    }
}
