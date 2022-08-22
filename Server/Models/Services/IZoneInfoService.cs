using PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.Services;

public interface IZoneInfoService
{
    Task<Zone?> GetZoneInfoAsync(string zoneName, CancellationToken token = default);


    // Maybe something like getZone direct, instead of just throwing errors up from bad http status codes, which works PDNS HTTP Remote but not very well for Rest API

    Task<List<Zone>?> GetAllZoneInfoAsync(bool includeDisabled, CancellationToken token = default);

    Task<IOperationResult> SetZoneInfoAsync(Zone newZoneInfo, CancellationToken token = default);


    Task<IOperationResult> DeleteZoneAsync(Zone zone, CancellationToken token = default);
}