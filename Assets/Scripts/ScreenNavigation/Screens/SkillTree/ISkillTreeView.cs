using System;
using System.Collections.Generic;
using Game.Services.Skills;
using UnityEngine.Events;

namespace Game.ScreenNavigation
{
    public interface ISkillTreeView
    {
        event UnityAction TabPressed;
        event Action Shown;
        event Action CycleRequested;
        event Action<string> Selected;
        event Action<SkillContext, string, int> PurchaseRequested;
        bool Visible { get; }
        void SelectTab();
        void ShowSelection(string id, string feedback = null);
        void Render(SkillContext context, SkillTreeDefinition tree, string selection,
            IReadOnlyDictionary<string, SkillOffer> offers, int cash, string feedback);
    }
}
