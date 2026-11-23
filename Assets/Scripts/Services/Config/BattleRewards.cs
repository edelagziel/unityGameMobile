using System;
using UnityEngine;

namespace Game.Services.Config
{
    /// <summary>Cash paid at the end of a battle. A loss pays a little, so a bad run still moves the meta.</summary>
    [Serializable]
    public sealed class BattleRewards
    {
        [Tooltip("Cash for a won battle.")]
        [SerializeField, Min(1)] private int win;

        [Tooltip("Cash for a lost battle. Not more than a win.")]
        [SerializeField, Min(0)] private int lose;

        /// <summary>Used by Unity when it creates the asset. The values come from the Inspector.</summary>
        public BattleRewards() { }

        public BattleRewards(int win, int lose)
        {
            this.win = win;
            this.lose = lose;
            Validate();
        }

        public int Win => win;
        public int Lose => lose;

        public int RewardFor(BattleOutcome outcome) => outcome switch
        {
            BattleOutcome.Win => win,
            BattleOutcome.Lose => lose,
            _ => throw new ArgumentOutOfRangeException(nameof(outcome), outcome, "No reward for this outcome.")
        };

        internal void Validate()
        {
            if (win < 1) throw new InvalidOperationException($"Battle rewards: a win must pay at least 1 (is {win}).");
            if (lose < 0) throw new InvalidOperationException($"Battle rewards: a loss can't be negative (is {lose}).");
            if (lose > win) throw new InvalidOperationException($"Battle rewards: a loss ({lose}) can't pay more than a win ({win}).");
        }
    }
}
