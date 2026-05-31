using GameVault.Models;

namespace GameVault.ViewModels;

public class AssetCardViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Platform Platform { get; set; }
    public int Year { get; set; }
    public Condition Condition { get; set; }
    public decimal EstimatedValue { get; set; }
    public string? ImageUrl { get; set; }
    public string OwnerDisplayName { get; set; } = string.Empty;
}
