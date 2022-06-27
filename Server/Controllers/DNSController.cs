using Microsoft.AspNetCore.Mvc;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Services;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Controllers;

// For the CLI tool to change values
[ApiController]
[Route("v1/dnsapi")]
public partial class DNSController : ControllerBase
{
    private readonly IRecordInfoService _recordInfoService;
    private readonly IZoneInfoService _zoneInfoService;

    public DNSController(IRecordInfoService recordInfoService, IZoneInfoService zoneInfoService)
    {
        _recordInfoService = recordInfoService;
        _zoneInfoService = zoneInfoService;
    }


    // Methods in Records and Zones files (DNSController.Records.cs, DNSController.Zones.cs)
}