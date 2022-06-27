using Microsoft.AspNetCore.Mvc;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.PowerDNS_Responses;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.PowerDNS_Responses.Lookup;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.PowerDNS_Responses.ZoneInfo;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.PowerDNS_Responses.ZoneMetadata;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Services;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Controllers;

[ApiController]
[Route("dns")]
public class PowerDNSController : ControllerBase
{
    private readonly IRecordInfoService _recordInfoService;
    private readonly IZoneInfoService _zoneInfoService;

    public PowerDNSController(IRecordInfoService recordInfoService, IZoneInfoService zoneInfoService)
    {
        _recordInfoService = recordInfoService;
        _zoneInfoService = zoneInfoService;
    }


    [HttpGet("lookup/{qname}/{qtype}")]
    public async Task<ActionResult<IDnsResponse>> Lookup(string qname, string qtype)
    {
        if (qtype.Equals("ANY", StringComparison.OrdinalIgnoreCase))
            return Ok(new LookupResponse(await _recordInfoService.ListRecordAsync(qname)));

        return Ok(new LookupResponse(await _recordInfoService.GetRecordAsync(qname, qtype)));
    }


    // Required
    [HttpGet("getAllDomainMetadata/{name}")]
    public ActionResult<IDnsResponse> GetAllDomainMetadata(string name)
    {
        return Ok(new ZoneMetaDataResponse());
    }


    [HttpGet("getDomainInfo/{name}")]
    public async Task<ActionResult<GetZoneInfoResponse>> GetDomainInfo(string name)
    {
        var zoneInfo = await _zoneInfoService.GetZoneInfoAsync(name);
        if (zoneInfo == null)
            // If we did want to handle this (to support the powerdns cli utility),
            // we would need to create the domain here, and still return none, and the cli will retry the request and get the newly created zone next time
            return NotFound(new ResultResponseNone());
        return Ok(new GetZoneInfoResponse(zoneInfo));
    }


    // Fill Zone Cache
    [HttpGet("getAllDomains")]
    public async Task<ActionResult<GetAllZoneInfoResponse>> GetAllDomains(bool includeDisabled)
    {
        return Ok(new GetAllZoneInfoResponse(await _zoneInfoService.GetAllZoneInfoAsync(includeDisabled)));
    }
}