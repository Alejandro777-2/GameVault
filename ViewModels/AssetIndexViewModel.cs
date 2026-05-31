using GameVault.Models;

namespace GameVault.ViewModels;

public class AssetIndexViewModel
{
    public List<AssetCardViewModel> Assets { get; set; } = [];
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public string? SearchTerm { get; set; }
    public Platform? PlatformFilter { get; set; }
    public Condition? ConditionFilter { get; set; }
}
