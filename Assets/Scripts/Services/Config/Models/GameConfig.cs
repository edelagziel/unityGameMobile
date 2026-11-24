using UnityEngine;

namespace Game.Services.Config
{
    [CreateAssetMenu(menuName = "Game/Game Config", fileName = "GameConfig")]
    public sealed class GameConfig : ScriptableObject
    {
        [SerializeField] private EconomyConfig economy;

        public EconomyConfig Economy => economy;
    }
}
