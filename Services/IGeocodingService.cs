namespace GameVault.Services;

public interface IGeocodingService
{
    Task<(double? Latitude, double? Longitude)> GeocodeCityAsync(string city);
}
