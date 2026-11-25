using System;
using UnityEngine;

namespace Game.Services.Skills
{
    [Serializable]
    public sealed class StatModifierEffect : NumericModifierEffect
    {
        [SerializeField] private SkillStat stat;
        public SkillStat Stat => stat;

        public override string TargetId => stat switch
        {
            SkillStat.ArmyHealth => "army.health",
            SkillStat.UnitHealth => "unit.health",
            SkillStat.UnitDamage => "unit.damage",
            SkillStat.UnitRange => "unit.range",
            SkillStat.UnitArmor => "unit.armor",
            SkillStat.TestBonus => "test.bonus",
            _ => null
        };
    }
}
