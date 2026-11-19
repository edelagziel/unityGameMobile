using System;

namespace Game.Services.Wallet
{
    /// <summary>
    /// The player's Cash: the meta currency earned after battles and spent on cards and Skill tree upgrades.
    /// </summary>
    public interface IWalletService
    {
        int Cash { get; }

        /// <summary>Raised after every change, with the new balance.</summary>
        event Action<int> CashChanged;

        void Add(int amount);

        /// <summary>Spends the amount if the player has enough Cash; otherwise changes nothing and returns false.</summary>
        bool TrySpend(int amount);
    }
}
