using GameVault.Models;

namespace GameVault.ViewModels;

public class UserProfileViewModel
{
    public ApplicationUser User { get; set; } = null!;
    public List<AssetCardViewModel> RecentAssets { get; set; } = [];
    public int TotalActiveAssets { get; set; }
}
