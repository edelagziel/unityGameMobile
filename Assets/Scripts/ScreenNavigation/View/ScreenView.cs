using UnityEngine;

namespace Game.ScreenNavigation
{
    public abstract class ScreenView : MonoBehaviour
    {
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}
