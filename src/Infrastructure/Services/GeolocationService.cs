// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

/// <summary>
/// Service for retrieving geolocation information using the ipapi.co API.
/// </summary>
public class GeolocationService : IGeolocationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeolocationService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeolocationService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client for making API requests.</param>
    /// <param name="logger">The logger instance.</param>
    public GeolocationService(HttpClient httpClient, ILogger<GeolocationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
    }

    /// <inheritdoc />
    public async Task<string?> GetCountryAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            _logger.LogWarning("IP address is null or empty");
            return null;
        }

        try
        {
            var url = $"http://ipapi.co/{ipAddress}/country/";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get country information. Status: {StatusCode}", response.StatusCode);
                return null;
            }

            var country = await response.Content.ReadAsStringAsync(cancellationToken);
            
            // API returns the country code as plain text
            country = country?.Trim();
            
            if (string.IsNullOrEmpty(country) || country.Equals("undefined", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Received invalid country response");
                return null;
            }

            return country;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed while getting country information");
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Request timeout while getting country information");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting country information");
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<GeolocationInfo?> GetGeolocationAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            _logger.LogWarning("IP address is null or empty");
            return null;
        }

        try
        {
            //ipAddress = "180.107.106.63";
            // Using HTTPS for secure communication
            var url = $"http://ip-api.com/json/{ipAddress}";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get geolocation information. Status: {StatusCode}", response.StatusCode);
                return null;
            }

            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                _logger.LogWarning("Received empty response");
                return null;
            }

            var apiResponse = JsonSerializer.Deserialize<IpApiComResponse>(jsonContent, _jsonOptions);
            
            if (apiResponse == null)
            {
                _logger.LogWarning("Failed to deserialize response");
                return null;
            }

            // Check if the API returned an error (status != "success")
            if (apiResponse.Status != "success")
            {
                _logger.LogWarning("API returned error status: {Status}", apiResponse.Status);
                return null;
            }

            var geolocation = new GeolocationInfo
            {
                IpAddress = ipAddress,
                Country = apiResponse.CountryCode,
                CountryName = apiResponse.Country,
                Region = apiResponse.RegionName,
                City = apiResponse.City,
                PostalCode = apiResponse.Zip,
                Latitude = apiResponse.Lat,
                Longitude = apiResponse.Lon,
                Timezone = apiResponse.Timezone,
                Isp = apiResponse.Isp,
                Organization = apiResponse.Org
            };

            return geolocation;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization failed");
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed while getting geolocation information");
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Request timeout while getting geolocation information");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting geolocation information");
            return null;
        }
    }

    /// <summary>
    /// Represents the response from the ip-api.com JSON API.
    /// </summary>
    private class IpApiComResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("countryCode")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("region")]
        public string? Region { get; set; }

        [JsonPropertyName("regionName")]
        public string? RegionName { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("zip")]
        public string? Zip { get; set; }

        [JsonPropertyName("lat")]
        public double? Lat { get; set; }

        [JsonPropertyName("lon")]
        public double? Lon { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("isp")]
        public string? Isp { get; set; }

        [JsonPropertyName("org")]
        public string? Org { get; set; }

        [JsonPropertyName("as")]
        public string? As { get; set; }

        [JsonPropertyName("query")]
        public string? Query { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    /// <summary>
    /// Represents the response from the ipapi.co JSON API.
    /// </summary>
    private class IpApiResponse
    {
        [JsonPropertyName("ip")]
        public string? Ip { get; set; }

        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("region")]
        public string? Region { get; set; }

        [JsonPropertyName("region_code")]
        public string? RegionCode { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("country_name")]
        public string? CountryName { get; set; }

        [JsonPropertyName("country_code")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("country_code_iso3")]
        public string? CountryCodeIso3 { get; set; }

        [JsonPropertyName("country_capital")]
        public string? CountryCapital { get; set; }

        [JsonPropertyName("country_tld")]
        public string? CountryTld { get; set; }

        [JsonPropertyName("continent_code")]
        public string? ContinentCode { get; set; }

        [JsonPropertyName("in_eu")]
        public bool? InEu { get; set; }

        [JsonPropertyName("postal")]
        public string? Postal { get; set; }

        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("utc_offset")]
        public string? UtcOffset { get; set; }

        [JsonPropertyName("country_calling_code")]
        public string? CountryCallingCode { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("currency_name")]
        public string? CurrencyName { get; set; }

        [JsonPropertyName("languages")]
        public string? Languages { get; set; }

        [JsonPropertyName("country_area")]
        public double? CountryArea { get; set; }

        [JsonPropertyName("country_population")]
        public long? CountryPopulation { get; set; }

        [JsonPropertyName("asn")]
        public string? Asn { get; set; }

        [JsonPropertyName("org")]
        public string? Org { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }
}
