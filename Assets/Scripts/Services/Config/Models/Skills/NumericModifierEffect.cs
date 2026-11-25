using System;
using UnityEngine;

namespace Game.Services.Skills
{
    // All currently authored modifiers are additive.
    [Serializable]
    public abstract class NumericModifierEffect : SkillEffectDefinition
    {
        [SerializeField] private float amount;
        public float Amount => amount;
        public override bool HasValidParameters => !float.IsNaN(amount) && !float.IsInfinity(amount);
    }
}
