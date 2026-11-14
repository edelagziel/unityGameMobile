using UnityEngine.SceneManagement;

namespace Game.ScreenNavigation
{
    public sealed class ScreenFlow : IScreenFlow
    {
        public const string HomeScene = "Home";
        public const string LevelScene = "Level";

        public void LoadHome() => SceneManager.LoadScene(HomeScene);
        public void LoadLevel() => SceneManager.LoadScene(LevelScene);
    }
}