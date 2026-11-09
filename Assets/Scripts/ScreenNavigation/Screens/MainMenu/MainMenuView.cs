using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ScreenNavigation
{
    public sealed class MainMenuView : ScreenView
    {
        [SerializeField] private Button settingsButton;
        public event Action SettingsPressed;

        private void Awake()
        {
            if (settingsButton == null)
            {
                Debug.LogError("MainMenuView needs a Settings button reference.", this);
                return;
            }
            settingsButton.onClick.AddListener(OnSettingsClicked);
        }

        private void OnDestroy()
        {
            if (settingsButton != null) settingsButton.onClick.RemoveListener(OnSettingsClicked);
        }

        private void OnSettingsClicked() => SettingsPressed?.Invoke();
    }
}
