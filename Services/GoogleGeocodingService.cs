using System.Text.Json;

namespace GameVault.Services;

public class GoogleGeocodingService : IGeocodingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string? _apiKey;
    private readonly ILogger<GoogleGeocodingService> _logger;

    public GoogleGeocodingService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<GoogleGeocodingService> logger)
    {
        _httpClientFactory = httpClientFactory;
        // Must read GeocodingApiKey (backend, unrestricted) — NOT ApiKey (frontend, referrer-restricted). Using the wrong key causes REQUEST_DENIED.
        _apiKey = configuration["GoogleMaps:GeocodingApiKey"];
        _logger = logger;
    }

    public async Task<(double? Latitude, double? Longitude)> GeocodeCityAsync(string city)
    {
        _logger.LogInformation("Geocoding requested for city: {City}", city);
        _logger.LogInformation("Geocoding API key is {KeyStatus}",
            string.IsNullOrWhiteSpace(_apiKey) ? "MISSING" : "present");

        if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(_apiKey))
            return (null, null);

        var addressQuery = city.Contains("ecuador", StringComparison.OrdinalIgnoreCase)
            ? city
            : city + ", Ecuador";

        var encodedAddress = Uri.EscapeDataString(addressQuery);
        var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={_apiKey}";

        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            var status = root.GetProperty("status").GetString();
            _logger.LogInformation("Geocoding API raw response status: {Status}", status);

            if (status != "OK")
            {
                _logger.LogWarning("Geocoding returned non-OK status '{Status}' for city '{City}'", status, city);
                return (null, null);
            }

            var results = root.GetProperty("results");
            if (results.GetArrayLength() == 0)
                return (null, null);

            var location = results[0]
                .GetProperty("geometry")
                .GetProperty("location");

            var lat = location.GetProperty("lat").GetDouble();
            var lng = location.GetProperty("lng").GetDouble();

            _logger.LogInformation("Geocoding succeeded: lat={Lat}, lng={Lng}", lat, lng);

            return (lat, lng);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Geocoding failed for city '{City}'", city);
            return (null, null);
        }
    }
}
