using System;
using UnityEngine;

namespace Game.Services.Skills
{
    [Serializable]
    public sealed class GameplayModifierEffect : NumericModifierEffect
    {
        [SerializeField] private string ruleId;
        public override string TargetId => ruleId;
    }
}
