namespace GameVault.ViewModels;

public class MapCollectorViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? City { get; set; }
    public int ActiveAssetCount { get; set; }
}
