using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.ScreenNavigation
{
    // A press may span an owner, selection or level change on a persistent button.
    internal sealed class SkillPurchaseButton : Button
    {
        private bool quoteChanged;
        public void InvalidatePress() => quoteChanged = true;

        public override void OnPointerDown(PointerEventData data)
        {
            quoteChanged = false;
            base.OnPointerDown(data);
        }

        public override void OnPointerClick(PointerEventData data)
        {
            if (!quoteChanged) base.OnPointerClick(data);
        }
    }
}
