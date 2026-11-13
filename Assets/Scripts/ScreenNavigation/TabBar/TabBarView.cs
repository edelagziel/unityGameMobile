using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ScreenNavigation
{
    public enum MenuTab { Shop = 0, Inventory = 1, Battle = 2, SkillTree = 3, Recipes = 4 }

    public sealed class TabBarView : MonoBehaviour
    {
        // Order: Shop, Inventory, Battle, SkillTree, Recipes
        [SerializeField] private Button[] tabButtons = new Button[5];
        [SerializeField] private GameObject[] highlights = new GameObject[5];

        public event Action<MenuTab> TabPressed;

        private void Awake()
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                if (tabButtons[i] == null)
                {
                    Debug.LogError($"TabBarView: tab button {i} is missing.", this);
                    continue;
                }
                var tab = (MenuTab)i;
                tabButtons[i].onClick.AddListener(() => TabPressed?.Invoke(tab));
            }
        }

        private void OnDestroy()
        {
            foreach (var button in tabButtons)
                if (button != null) button.onClick.RemoveAllListeners();
        }

        public void ShowSelected(MenuTab tab)
        {
            for (int i = 0; i < highlights.Length; i++)
                if (highlights[i] != null) highlights[i].SetActive(i == (int)tab);
        }
    }
}