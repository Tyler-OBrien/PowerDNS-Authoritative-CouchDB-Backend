using System.Net;
using PowerDNS_Auth_CouchDB_Remote_Backend.Brokers;
using PowerDNS_Auth_CouchDB_Remote_Backend.Extensions.HTTPClient;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Services;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Services;

public class ZoneInfoService : IZoneInfoService
{
    private readonly IAPIBroker _apiBroker;


    public ZoneInfoService(IAPIBroker apiBroker)
    {
        _apiBroker = apiBroker;
    }


    public async Task<Zone?> GetZoneInfoAsync(string zoneName, CancellationToken token = default)
    {
        return await _apiBroker.GetZoneInfoAsync(zoneName, token);
    }

    public async Task<List<Zone>?> GetAllZoneInfoAsync(bool includeDisabled, CancellationToken token = default)
    {
        return await _apiBroker.GetAllZoneInfoAsync(includeDisabled, token);
    }

    public async Task<IOperationResult> SetZoneInfoAsync(Zone newZoneInfo, CancellationToken token = default)
    {
        var response = await _apiBroker.SetZoneInfoAsync(newZoneInfo, token);
        if (response.IsSuccessStatusCode)
            return new GenericOperationResult<CouchDbOperationResult>(true, $"Created {newZoneInfo.ID}",
                HttpStatusCode.OK, await response.GetCouchDBOperationResult());
        if (response.StatusCode == HttpStatusCode.Conflict)
            return new GenericOperationErrorResult(false, $"{newZoneInfo.ID} already exists...",
                HttpStatusCode.Conflict); // Already Exists!
        // If anything else, throw and let the error handler middleware deal with it.
        response.EnsureSuccessStatusCode();
        // Error will be thrown before this anyway
        return new GenericOperationErrorResult(false, "Internal Error", HttpStatusCode.InternalServerError);
    }

    public async Task<IOperationResult> DeleteZoneAsync(Zone zone, CancellationToken token = default)
    {
        if (zone.ID == null)
            return new GenericOperationErrorResult(false, "Zone ID cannot be null", HttpStatusCode.BadRequest);


        var response = await _apiBroker.DeleteZoneAsync(zone, token);
        if (response.IsSuccessStatusCode)
            return new GenericOperationResult<CouchDbOperationResult>(true, $"Successfully deleted {zone}",
                HttpStatusCode.OK, await response.GetCouchDBOperationResult());
        if (response.StatusCode == HttpStatusCode.Conflict)
            return new GenericOperationErrorResult(false, "Document Conflict Error", HttpStatusCode.Conflict);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return new GenericOperationErrorResult(false, "Document not found", HttpStatusCode.NotFound);
        // If anything else, throw and let the error handler middleware deal with it.
        response.EnsureSuccessStatusCode();
        // Error will be thrown before this anyway
        return new GenericOperationErrorResult(false, "Internal Error", HttpStatusCode.InternalServerError);
    }
}