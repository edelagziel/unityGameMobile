using System;
using UnityEngine;

namespace Game.Services.Skills
{
    [Serializable]
    public sealed class SkillRequirement
    {
        [SerializeField] private string skillId;
        [SerializeField] private int requiredLevel;

        public string SkillId => skillId;
        public int RequiredLevel => requiredLevel;
    }
}
