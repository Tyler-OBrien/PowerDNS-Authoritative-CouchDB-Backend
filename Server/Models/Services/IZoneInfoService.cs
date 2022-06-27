using PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.Services;

public interface IZoneInfoService
{
    Task<Zone?> GetZoneInfoAsync(string zoneName);


    // Maybe something like getZone direct, instead of just throwing errors up from bad http status codes, which works PDNS HTTP Remote but not very well for Rest API

    Task<List<Zone>?> GetAllZoneInfoAsync(bool includeDisabled);

    Task<IOperationResult> SetZoneInfoAsync(Zone newZoneInfo);


    Task<IOperationResult> DeleteZoneAsync(Zone zone);
}