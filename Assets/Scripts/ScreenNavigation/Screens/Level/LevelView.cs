using System;
using Game.Services.Config;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ScreenNavigation
{
    public sealed class LevelView : ScreenView
    {
        [SerializeField] private Button backButton;

        [Header("Stand-ins until the real battle can end itself")]
        [SerializeField] private Button winButton;
        [SerializeField] private Button loseButton;

        [Header("End of the battle")]
        [SerializeField] private BattleResultPanel resultPanel;

        public event Action BackPressed;

        /// <summary>The battle ended. Raised by the stand-in Win and Lose buttons for now.</summary>
        public event Action<BattleOutcome> Finished;

        public event Action BackToMenuPressed
        {
            add { if (resultPanel != null) resultPanel.BackToMenuPressed += value; }
            remove { if (resultPanel != null) resultPanel.BackToMenuPressed -= value; }
        }

        public bool IsResultShown => resultPanel != null && resultPanel.IsShown;

        private void Awake()
        {
            if (backButton == null || winButton == null || loseButton == null || resultPanel == null)
            {
                Debug.LogError("LevelView needs the Back, Win and Lose buttons and the Result panel.", this);
                return;
            }
            backButton.onClick.AddListener(OnBackClicked);
            winButton.onClick.AddListener(OnWinClicked);
            loseButton.onClick.AddListener(OnLoseClicked);
        }

        private void OnDestroy()
        {
            if (backButton != null) backButton.onClick.RemoveListener(OnBackClicked);
            if (winButton != null) winButton.onClick.RemoveListener(OnWinClicked);
            if (loseButton != null) loseButton.onClick.RemoveListener(OnLoseClicked);
        }

        /// <summary>Every new battle starts without the last battle's result on top.</summary>
        public override void Show()
        {
            if (resultPanel != null) resultPanel.Hide();
            base.Show();
        }

        public void ShowResult(BattleOutcome outcome, int reward) => resultPanel.Show(outcome, reward);

        private void OnBackClicked() => BackPressed?.Invoke();
        private void OnWinClicked() => Finished?.Invoke(BattleOutcome.Win);
        private void OnLoseClicked() => Finished?.Invoke(BattleOutcome.Lose);
    }
}
