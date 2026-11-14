using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ScreenNavigation
{
    public sealed class LevelView : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        public event Action BackPressed;

        private void Awake()
        {
            if (backButton == null)
            {
                Debug.LogError("LevelView needs a Back button reference.", this);
                return;
            }
            backButton.onClick.AddListener(OnBackClicked);
        }

        private void OnDestroy()
        {
            if (backButton != null) backButton.onClick.RemoveListener(OnBackClicked);
        }

        private void OnBackClicked() => BackPressed?.Invoke();
    }
}