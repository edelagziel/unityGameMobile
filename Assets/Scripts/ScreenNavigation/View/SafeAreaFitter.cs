using UnityEngine;

namespace Game.ScreenNavigation
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        private void Awake()
        {
            var rect = GetComponent<RectTransform>();
            var safe = Screen.safeArea;
            var min = safe.position;
            var max = safe.position + safe.size;
            min.x /= Screen.width; min.y /= Screen.height;
            max.x /= Screen.width; max.y /= Screen.height;
            rect.anchorMin = min;
            rect.anchorMax = max;
        }
    }
}