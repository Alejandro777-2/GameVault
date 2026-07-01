using GameVault.Models;

namespace GameVault.ViewModels;

public class MarketplaceDetailsViewModel
{
    public TradeOffer Offer { get; set; } = null!;
    public string OwnerEmail { get; set; } = string.Empty;
}
