using Microsoft.AspNetCore.Mvc;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.API_Responses;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Controllers;

public partial class DNSController
{
    [HttpGet("Zone/List")]
    public async Task<ActionResult<IResponse>> GetZones([FromQuery] bool? includeDisabled = false, CancellationToken token = default)
    {
        var zoneInfo = await _zoneInfoService.GetAllZoneInfoAsync(includeDisabled ?? false, token);
        if (zoneInfo != null) return Ok(new DataResponse<List<Zone>>(zoneInfo));
        return Ok(new DataResponse<List<Zone>>(new List<Zone>()));
    }


    [HttpGet("Zone/{zone}")]
    public async Task<ActionResult<IResponse>> GetZone(string zone, CancellationToken token = default)
    {
        var zoneInfo = await _zoneInfoService.GetZoneInfoAsync(zone, token);
        if (zoneInfo != null) return Ok(new DataResponse<Zone>(zoneInfo));
        return NotFound(new ErrorResponse(404, $"Zone with name {zone} not found.", "zone_not_found"));
    }

    [HttpPost("Zone")]
    public async Task<ActionResult<IResponse>> NewZone([FromBody] Zone zone, CancellationToken token = default)
    {
        var tryCreate = await _zoneInfoService.SetZoneInfoAsync(zone, token);
        // We return accepted because one couchdb node accepting doesn't mean the others will...
        if (tryCreate.Success && tryCreate is GenericOperationResult<CouchDbOperationResult> tryCreateResult)
            return Accepted(new DataResponse<CouchDbOperationResult>(tryCreateResult.Data));
        return StatusCode(tryCreate.Code,
            new ErrorResponseDetails<IOperationResult>(tryCreate.Code, $"Failed to create zone - {tryCreate.Message}",
                "zone_creation_error", tryCreate));
    }

    [HttpDelete("Zone")]
    public async Task<ActionResult<IResponse>> DeleteZone(Zone zone, CancellationToken token = default)
    {
        var tryDelete = await _zoneInfoService.DeleteZoneAsync(zone, token);
        if (tryDelete.Success && tryDelete is GenericOperationResult<CouchDbOperationResult> tryDeleteResult)
            return Accepted(new DataResponse<CouchDbOperationResult>(tryDeleteResult.Data));
        return StatusCode(tryDelete.Code,
            new ErrorResponseDetails<IOperationResult>(tryDelete.Code, $"Failed to create record - {tryDelete.Message}",
                "zone_deletion_errors", tryDelete));
    }
}