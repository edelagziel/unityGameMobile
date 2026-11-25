using UnityEngine;

namespace Game.Services.Config
{
    [CreateAssetMenu(menuName = "Game/Game Config", fileName = "GameConfig")]
    public sealed class GameConfig : ScriptableObject
    {
        [SerializeField] private EconomyConfig economy;

        [SerializeField] private Game.Services.Skills.SkillTreeDefinition[] skillTrees;
        public System.Collections.Generic.IReadOnlyList<Game.Services.Skills.SkillTreeDefinition> SkillTrees => skillTrees;

        public EconomyConfig Economy => economy;
    }
}
