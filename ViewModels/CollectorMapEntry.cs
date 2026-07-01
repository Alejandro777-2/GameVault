using GameVault.Models;

namespace GameVault.ViewModels;

public class CollectorMapEntry
{
    public ApplicationUser User { get; set; } = null!;
    public int ActiveAssetCount { get; set; }
    public List<Platform> TopPlatforms { get; set; } = [];
}
