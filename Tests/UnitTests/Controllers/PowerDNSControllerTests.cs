using System.Net;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PowerDNS_Auth_CouchDB_Remote_Backend.Controllers;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.PowerDNS_Responses.Lookup;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.PowerDNS_Responses.ZoneInfo;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.PowerDNS_Responses.ZoneMetadata;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Services;

namespace UnitTests.Controllers;

public class PowerDNSControllerTests
{
    private readonly PowerDNSController _controller;
    private readonly Mock<IRecordInfoService> _mockRecordInfoService;
    private readonly Mock<IZoneInfoService> _mockZoneInfoService;
    private readonly IRecordInfoService _recordInfoService;

    private readonly IZoneInfoService _zoneInfoService;

    // Note to self: For each test, it creates a new instance (so old Mocks won't carry over).
    public PowerDNSControllerTests()
    {
        _mockZoneInfoService = new Mock<IZoneInfoService>(MockBehavior.Strict);
        _mockRecordInfoService = new Mock<IRecordInfoService>(MockBehavior.Strict);

        _zoneInfoService = _mockZoneInfoService.Object;
        _recordInfoService = _mockRecordInfoService.Object;

        _controller = new PowerDNSController(_recordInfoService, _zoneInfoService);
    }

    [Test]
    [AutoData]
    public async Task Lookup(List<Record> records)
    {
        // Data
        var qname = "example.com.";
        var qtype = "A";


        // Arrange
        _mockRecordInfoService.Setup(service => service.GetRecordAsync(qname, qtype, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(records);

        // Act
        var response = await _controller.Lookup(qname, qtype);


        //Assert
        var rightResponse = new LookupResponse(records);


        var okObjectResult = response.Result as OkObjectResult;


        okObjectResult.Should().NotBeNull("Ok Object Result should be returned");

        okObjectResult.Value.Should().BeEquivalentTo(rightResponse);
        okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK,
            "Service should return 200 - OK when returning data");
    }

    [Test]
    [AutoData]
    public async Task LookupAny(List<Record> records)
    {
        // Data
        var qname = "example.com.";
        var qtype = "ANY";


        // Arrange
        _mockRecordInfoService.Setup(service => service.ListRecordAsync(qname, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(records);

        // Act
        var response = await _controller.Lookup(qname, qtype);


        //Assert
        var rightResponse = new LookupResponse(records);


        var okObjectResult = response.Result as OkObjectResult;


        okObjectResult.Should().NotBeNull("Ok Object Result should be returned");

        okObjectResult.Value.Should().BeEquivalentTo(rightResponse);
        okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK,
            "Service should return 200 - OK when returning data");
    }


    [Test]
    [AutoData]
    // Not actually implemented behind just returning something to sastify powerdns
    public async Task getAllDomainMetadata(List<Record> records)
    {
        // Data


        // Arrange


        // Act
        var response = _controller.GetAllDomainMetadata("");


        //Assert
        var rightResponse = new ZoneMetaDataResponse();

        var okObjectResult = response.Result as OkObjectResult;


        okObjectResult.Should().NotBeNull("Ok Object Result should be returned");

        okObjectResult.Value.Should().BeEquivalentTo(rightResponse);
        okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK,
            "Service should return 200 - OK when returning data");
    }


    [Test]
    [AutoData]
    public async Task GetDomainInfo(Zone zone, string qname)
    {
        // Data


        // Arrange
        _mockZoneInfoService.Setup(service => service.GetZoneInfoAsync(qname, It.IsAny<CancellationToken>())).ReturnsAsync(zone);

        // Act
        var response = await _controller.GetDomainInfo(qname);


        //Assert
        var rightResponse = new GetZoneInfoResponse(zone);


        var okObjectResult = response.Result as OkObjectResult;


        okObjectResult.Should().NotBeNull("Ok Object Result should be returned");

        okObjectResult.Value.Should().BeEquivalentTo(rightResponse);
        okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK,
            "Service should return 200 - OK when returning data");
    }

    [Test]
    [AutoData]
    public async Task GetAllDomains(List<Zone> zones)
    {
        // Data
        var includeDisabled = false;


        // Arrange
        _mockZoneInfoService.Setup(service => service.GetAllZoneInfoAsync(includeDisabled, It.IsAny<CancellationToken>())).ReturnsAsync(zones);

        // Act
        var response = await _controller.GetAllDomains(includeDisabled);


        //Assert
        var rightResponse = new GetAllZoneInfoResponse(zones);


        var okObjectResult = response.Result as OkObjectResult;


        okObjectResult.Should().NotBeNull("Ok Object Result should be returned");

        okObjectResult.Value.Should().BeEquivalentTo(rightResponse);
        okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK,
            "Service should return 200 - OK when returning data");
    }
}