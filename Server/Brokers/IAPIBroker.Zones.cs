using PowerDNS_Auth_CouchDB_Remote_Backend.Models;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Brokers;

public partial interface IAPIBroker
{
    Task<Zone?> GetZoneInfoAsync(string zoneName);

    Task<List<Zone>?> GetAllZoneInfoAsync(bool includeDisabled);

    Task<HttpResponseMessage> SetZoneInfoAsync(Zone newZoneInfo);

    Task<HttpResponseMessage> DeleteZoneAsync(Zone zone);
}