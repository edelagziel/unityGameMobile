using System;

namespace Game.Services.Skills
{
    [Serializable]
    public abstract class SkillEffectDefinition
    {
        public abstract string TargetId { get; }
        public virtual bool HasValidParameters => true;
    }
}
