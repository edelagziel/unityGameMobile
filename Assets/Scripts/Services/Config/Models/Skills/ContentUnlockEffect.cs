using System;
using UnityEngine;

namespace Game.Services.Skills
{
    [Serializable]
    public sealed class ContentUnlockEffect : SkillEffectDefinition
    {
        [SerializeField] private SkillContentType contentType;
        [SerializeField] private string contentId;
        public SkillContentType ContentType => contentType;
        public override string TargetId => contentId;
        public override bool HasValidParameters => Enum.IsDefined(typeof(SkillContentType), contentType);
    }
}
