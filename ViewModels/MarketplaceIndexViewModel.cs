using GameVault.Models;

namespace GameVault.ViewModels;

public class MarketplaceIndexViewModel
{
    public List<TradeOfferCardViewModel> Offers { get; set; } = [];
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public Platform? PlatformFilter { get; set; }
    public TradeType? TypeFilter { get; set; }
}

public class TradeOfferCardViewModel
{
    public int Id { get; set; }
    public int AssetId { get; set; }
    public string AssetTitle { get; set; } = string.Empty;
    public string? AssetImageUrl { get; set; }
    public Platform AssetPlatform { get; set; }
    public Condition AssetCondition { get; set; }
    public TradeType Type { get; set; }
    public decimal? Price { get; set; }
    public string OwnerDisplayName { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
