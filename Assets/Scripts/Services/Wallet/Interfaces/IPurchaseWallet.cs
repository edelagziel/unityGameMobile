namespace Game.Services.Wallet
{
    // Cash and the purchased quantity are committed in the same wallet snapshot.
    public interface IPurchaseWallet : IWalletService
    {
        int Purchased(string product);
        bool TryPurchase(string product, int targetQuantity, int cashPrice);
    }
}
