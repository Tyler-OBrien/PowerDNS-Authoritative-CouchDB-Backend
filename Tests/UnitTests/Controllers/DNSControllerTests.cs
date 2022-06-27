using Moq;
using PowerDNS_Auth_CouchDB_Remote_Backend.Controllers;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Services;

namespace UnitTests.Controllers;

public partial class DNSControllerTests
{
    private readonly DNSController _controller;
    private readonly Mock<IRecordInfoService> _mockRecordInfoService;
    private readonly Mock<IZoneInfoService> _mockZoneInfoService;
    private readonly IRecordInfoService _recordInfoService;

    private readonly IZoneInfoService _zoneInfoService;

    // Note to self: For each test, it creates a new instance (so old Mocks won't carry over).
    public DNSControllerTests()
    {
        _mockZoneInfoService = new Mock<IZoneInfoService>(MockBehavior.Strict);
        _mockRecordInfoService = new Mock<IRecordInfoService>(MockBehavior.Strict);

        _zoneInfoService = _mockZoneInfoService.Object;
        _recordInfoService = _mockRecordInfoService.Object;

        _controller = new DNSController(_recordInfoService, _zoneInfoService);
    }
}