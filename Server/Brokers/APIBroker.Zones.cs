using PowerDNS_Auth_CouchDB_Remote_Backend.Extensions.HTTPClient;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Brokers;

public partial class APIBroker : IAPIBroker
{
    public readonly string ZonesDB;

    public async Task<Zone?> GetZoneInfoAsync(string zoneName, CancellationToken token)
    {
        return await _httpClient.GetFromJsonAsyncSupportNull<Zone>("/" + ZonesDB + "/" +
                                                                   Uri.EscapeDataString(zoneName), token);
    }

    public async Task<List<Zone>?> GetAllZoneInfoAsync(bool includeDisabled, CancellationToken token)
    {
        return await _httpClient.CouchDBViewGetFromJsonAsyncSupportNull<Zone>("/" + ZonesDB + "/" +
                                                                              "_all_docs?include_docs=true&limit=2500", token); // Built in view
    }

    public async Task<HttpResponseMessage> SetZoneInfoAsync(Zone newZoneInfo, CancellationToken token)
    {
        return await _httpClient.PostAsJsonAsync($"/{ZonesDB}/",
            newZoneInfo, token);
    }

    public async Task<HttpResponseMessage> DeleteZoneAsync(Zone zone, CancellationToken token)
    {
        // We know ID won't be null
        return await _httpClient.DeleteAsync($"/{ZonesDB}/{Uri.EscapeDataString(zone.ID)}?rev={zone.Revision}", token);
    }
}