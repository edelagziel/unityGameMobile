using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Services.Skills
{
    [CreateAssetMenu(menuName = "Game/Skill Tree")]
    public sealed class SkillTreeDefinition : ScriptableObject
    {
        [Tooltip("Persistent ID. Keep it stable after release. Letters, digits, dots, dashes and underscores.")]
        [SerializeField] private string id;
        [SerializeField] private string title;
        [Tooltip("Minimum clearance between radial node footprints, in Canvas units.")]
        [SerializeField, Min(0)] private float nodeSpacing = 32;
        [Tooltip("Minimum center-to-center distance between successive rings.")]
        [SerializeField, Min(1)] private float ringSpacing = 320;
        [SerializeField] private SkillDefinition[] skills = Array.Empty<SkillDefinition>();

        public string Id => id;
        public string Title => title;
        // Nested Inspector edits also invalidate positions, captions and connection thresholds.
        public int DefinitionRevision { get; private set; }
        private void OnValidate() => DefinitionRevision++;

        public float NodeSpacing => nodeSpacing;
        public float RingSpacing => ringSpacing;
        public IReadOnlyList<SkillDefinition> Skills => skills;
    }
}
