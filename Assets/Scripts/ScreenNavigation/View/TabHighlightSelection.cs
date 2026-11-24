using UnityEngine;

namespace Game.ScreenNavigation
{
    public sealed class TabHighlightSelection
    {
        private GameObject selected;

        public void Select(GameObject highlight)
        {
            if (selected == highlight) return;
            if (selected != null) selected.SetActive(false);
            highlight.SetActive(true);
            selected = highlight;
        }
    }
}
