using UnityEditor;
using UnityEditor.SceneManagement;

namespace Game.EditorTools
{
    /// <summary>
    /// Editor only, never in the game: every Play starts in the Bootstrap scene, like a launch on the phone,
    /// so the services always exist. When Play stops, Unity reopens the scene you were editing.
    /// </summary>
    [InitializeOnLoad]
    internal static class PlayFromBootstrap
    {
        private const string BootstrapScenePath = "Assets/Scenes/Bootstrap.unity";

        static PlayFromBootstrap()
        {
            Apply();
            EditorApplication.projectChanged += Apply; // Picks the scene up as soon as it is created.
        }

        private static void Apply() =>
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootstrapScenePath);
    }
}
