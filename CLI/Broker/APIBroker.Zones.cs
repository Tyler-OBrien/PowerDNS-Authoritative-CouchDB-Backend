using System.Net.Http.Json;
using CLI.Extensions;
using CLI.Models.DTOs;

namespace CLI.Broker;

public partial class APIBroker
{
    public const string ZonesDB = "/v1/dnsapi/Zone";

    public async Task<HttpResponseMessage> GetZoneInfoAsync(string zoneName)
    {
        return await _httpClient.GetAsync(ZonesDB + "/" +
                                          Uri.EscapeDataString(NormalizeZoneNames(zoneName)));
    }

    public async Task<HttpResponseMessage> GetAllZoneInfoAsync(bool includeDisabled)
    {
        return await _httpClient.GetAsync(ZonesDB + $"/List?includeDisabled={includeDisabled}");
    }

    public async Task<HttpResponseMessage> SetZoneInfoAsync(Zone newZoneInfo)
    {
        return await _httpClient.PostAsJsonAsync($"{ZonesDB}",
            newZoneInfo);
    }

    public async Task<HttpResponseMessage> DeleteZoneAsync(Zone zone)
    {
        return await _httpClient.DeleteAsJsonAsync($"{ZonesDB}", zone);
    }

    internal string NormalizeZoneNames(string input)
    {
        if (input.EndsWith(".")) return input.TrimEnd('.');

        return input;
    }
}