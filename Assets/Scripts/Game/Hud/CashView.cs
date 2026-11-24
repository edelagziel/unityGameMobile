using TMPro;
using UnityEngine;

namespace Game.Hud
{
    /// <summary>Shows the player's Cash. It does not know where Cash comes from (battles pay it, see BattleRewardGranter).</summary>
    public sealed class CashView : MonoBehaviour
    {
        [SerializeField] private TMP_Text cashLabel;

        private void Awake()
        {
            if (cashLabel == null) Debug.LogError("CashView needs the Cash label.", this);
        }

        public void ShowCash(int cash)
        {
            if (cashLabel != null) cashLabel.text = cash.ToString();
        }
    }
}
