using System;
using UnityEngine;

namespace Game.Services.Config
{
    /// <summary>
    /// Cash paid at the end of a battle. A loss pays a little, so a bad run still moves the meta.
    /// Plain data from the Inspector (a struct: never null, nothing to create).
    /// </summary>
    [Serializable]
    public struct BattleRewards
    {
        [Tooltip("Cash for a won battle.")]
        [SerializeField, Min(1)] private int win;

        [Tooltip("Cash for a lost battle. Not more than a win.")]
        [SerializeField, Min(0)] private int lose;

        public BattleRewards(int win, int lose)
        {
            this.win = win;
            this.lose = lose;
        }

        public int Win => win;
        public int Lose => lose;

        /// <summary>A win pays at least 1; a loss pays from 0 up to a win.</summary>
        public bool IsValid => win >= 1 && lose >= 0 && lose <= win;

        /// <summary>The reward for this outcome. An outcome without a reward pays nothing instead of stopping the game.</summary>
        public int RewardFor(BattleOutcome outcome) => outcome switch
        {
            BattleOutcome.Win => win,
            BattleOutcome.Lose => lose,
            _ => 0
        };
    }
}
