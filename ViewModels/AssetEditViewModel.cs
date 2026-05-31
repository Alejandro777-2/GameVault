using System.ComponentModel.DataAnnotations;

namespace GameVault.ViewModels;

public class AssetEditViewModel : AssetCreateViewModel
{
    [Required]
    public int Id { get; set; }

    public string? ExistingImageUrl { get; set; }
}
