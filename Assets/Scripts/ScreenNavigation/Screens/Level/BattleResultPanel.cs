using System;
using Game.Services.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ScreenNavigation
{
    /// <summary>
    /// The end of a battle, shown over the dimmed battle (a black, half-transparent panel inside LevelView):
    /// the outcome, the Cash it paid, and the way back to the menu. While it is shown it blocks the battle's buttons.
    /// </summary>
    public sealed class BattleResultPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleLabel;
        [SerializeField] private TMP_Text rewardLabel;
        [SerializeField] private Button backToMenuButton;

        [Header("Texts")]
        [SerializeField] private string winTitle = "Victory!";
        [SerializeField] private string loseTitle = "Defeat";

        public event Action BackToMenuPressed;

        public bool IsShown => gameObject.activeSelf;

        // The panel starts hidden, so this runs the first time it is shown, before any tap on it.
        private void Awake()
        {
            if (titleLabel == null || rewardLabel == null || backToMenuButton == null)
            {
                Debug.LogError("BattleResultPanel needs the Title and Reward labels and the Back to menu button.", this);
                return;
            }
            backToMenuButton.onClick.AddListener(OnBackToMenuClicked);
        }

        private void OnDestroy()
        {
            if (backToMenuButton != null) backToMenuButton.onClick.RemoveListener(OnBackToMenuClicked);
        }

        public void Show(BattleOutcome outcome, int reward)
        {
            if (titleLabel != null) titleLabel.text = outcome == BattleOutcome.Win ? winTitle : loseTitle;
            if (rewardLabel != null) rewardLabel.text = $"+{reward}";
            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);

        private void OnBackToMenuClicked() => BackToMenuPressed?.Invoke();
    }
}
