using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ScreenNavigation
{
    public sealed class MainMenuView : ScreenView
    {
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button playButton;
        public event Action SettingsPressed;
        public event Action PlayPressed;

        private void Awake()
        {
            if (settingsButton == null || playButton == null)
            {
                Debug.LogError("MainMenuView needs Settings and Play button references.", this);
                return;
            }
            settingsButton.onClick.AddListener(OnSettingsClicked);
            playButton.onClick.AddListener(OnPlayClicked);
        }

        private void OnDestroy()
        {
            if (settingsButton != null) settingsButton.onClick.RemoveListener(OnSettingsClicked);
            if (playButton != null) playButton.onClick.RemoveListener(OnPlayClicked);
        }

        private void OnSettingsClicked() => SettingsPressed?.Invoke();
        private void OnPlayClicked() => PlayPressed?.Invoke();
    }
}