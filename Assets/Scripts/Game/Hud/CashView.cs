using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Hud
{
    /// <summary>Shows the player's Cash. It does not know where Cash comes from.</summary>
    public sealed class CashView : MonoBehaviour
    {
        [SerializeField] private TMP_Text cashLabel;
        [SerializeField] private Button addCashButton; // Greybox: stands in for battle rewards until battles pay Cash.

        public event Action AddCashPressed;

        private void Awake()
        {
            if (cashLabel == null) Debug.LogError("CashView needs the Cash label.", this);
            if (addCashButton != null) addCashButton.onClick.AddListener(OnAddCashClicked);
        }

        private void OnDestroy()
        {
            if (addCashButton != null) addCashButton.onClick.RemoveListener(OnAddCashClicked);
        }

        public void ShowCash(int cash)
        {
            if (cashLabel != null) cashLabel.text = cash.ToString();
        }

        private void OnAddCashClicked() => AddCashPressed?.Invoke();
    }
}
