using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VContainer;

namespace Game.ScreenNavigation
{
    public abstract class TabbedScreenView : ScreenView
    {
        [SerializeField] private Button tabButton;
        [SerializeField] private GameObject tabHighlight;
        private TabHighlightSelection selection;

        [Inject]
        public void InitializeTab(TabHighlightSelection selection)
        {
            if (tabButton == null || tabHighlight == null)
                throw new InvalidOperationException($"Assign the tab button and highlight on {name}.");
            this.selection = selection;
            tabHighlight.SetActive(false);
        }

        // The button lives on the visible bar even when this screen is inactive.
        public event UnityAction TabPressed
        {
            add => tabButton.onClick.AddListener(value);
            remove { if (tabButton != null) tabButton.onClick.RemoveListener(value); }
        }

        public event Action Shown;

        public override void Show()
        {
            base.Show();
            Shown?.Invoke();
        }

        public void SelectTab() => selection.Select(tabHighlight);
    }
}
