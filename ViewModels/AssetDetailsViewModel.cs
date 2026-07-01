using GameVault.Models;

namespace GameVault.ViewModels;

public class AssetDetailsViewModel
{
    public Asset Asset { get; set; } = null!;
    public string OwnerDisplayName { get; set; } = string.Empty;
    public string? OwnerCity { get; set; }
    public string? OwnerCountry { get; set; }
    public bool IsCurrentUserOwner { get; set; }
    public bool IsInWishlist { get; set; }
    public bool HasActiveTradeOffer { get; set; }
}
