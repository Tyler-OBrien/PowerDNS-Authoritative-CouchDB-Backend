using PowerDNS_Auth_CouchDB_Remote_Backend.Models;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Brokers;

public partial interface IAPIBroker
{
    Task<Zone?> GetZoneInfoAsync(string zoneName, CancellationToken token);

    Task<List<Zone>?> GetAllZoneInfoAsync(bool includeDisabled, CancellationToken token);

    Task<HttpResponseMessage> SetZoneInfoAsync(Zone newZoneInfo, CancellationToken token);

    Task<HttpResponseMessage> DeleteZoneAsync(Zone zone, CancellationToken token);
}