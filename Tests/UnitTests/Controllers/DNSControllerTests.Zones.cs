using System.Net;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.API_Responses;

namespace UnitTests.Controllers;

public partial class DNSControllerTests
{
    [Test]
    [AutoData]
    public async Task GetZones(List<Zone> zones)
    {
        // Data
        var includeDisabled = true;

        // Arrange
        _mockZoneInfoService.Setup(service => service.GetAllZoneInfoAsync(includeDisabled)).ReturnsAsync(zones);

        // Act
        var response = await _controller.GetZones(includeDisabled);


        //Assert
        var rightResponse = new DataResponse<List<Zone>>(zones);


        var okObjectResult = response.Result as OkObjectResult;


        okObjectResult.Should().NotBeNull("Ok Object Result should be returned");

        okObjectResult.Value.Should().BeEquivalentTo(rightResponse);
        okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK,
            "Service should return 200 - OK when returning data");
    }

    [Test]
    [AutoData]
    public async Task GetZone(Zone zone, string zoneName)
    {
        // Data

        // Arrange
        _mockZoneInfoService.Setup(service => service.GetZoneInfoAsync(zoneName)).ReturnsAsync(zone);

        // Act
        var response = await _controller.GetZone(zoneName);


        //Assert
        var rightResponse = new DataResponse<Zone>(zone);


        var okObjectResult = response.Result as OkObjectResult;


        okObjectResult.Should().NotBeNull("Ok Object Result should be returned");

        okObjectResult.Value.Should().BeEquivalentTo(rightResponse);
        okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK,
            "Service should return 200 - OK when returning data");
    }


    [Test]
    [AutoData]
    public async Task NewZone(Zone newZone, CouchDbOperationResult result)
    {
        // Data
        var apiresponse = new GenericOperationResult<CouchDbOperationResult>(true, "", HttpStatusCode.OK, result);
        var controllerResponse = new DataResponse<CouchDbOperationResult>(result);

        // Arrange
        _mockZoneInfoService.Setup(service => service.SetZoneInfoAsync(newZone)).ReturnsAsync(apiresponse);

        // Act
        var response = await _controller.NewZone(newZone);


        //Assert

        var okObjectResult = response.Result as AcceptedResult;


        okObjectResult.Should().NotBeNull("Ok Object Result should be returned");


        okObjectResult.Value.Should().BeEquivalentTo(controllerResponse);
        okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.Accepted,
            "Service should return 202 - Accepted when setting data");
    }

    [Test]
    [AutoData]
    public async Task DeleteZone(Zone zone, CouchDbOperationResult result)
    {
        // Data
        var apiresponse = new GenericOperationResult<CouchDbOperationResult>(true, "", HttpStatusCode.OK, result);
        var controllerResponse = new DataResponse<CouchDbOperationResult>(result);

        // Arrange
        _mockZoneInfoService.Setup(service => service.DeleteZoneAsync(zone)).ReturnsAsync(apiresponse);

        // Act
        var response = await _controller.DeleteZone(zone);


        //Assert

        var okObjectResult = response.Result as AcceptedResult;


        okObjectResult.Should().NotBeNull("Ok Object Result should be returned");


        okObjectResult.Value.Should().BeEquivalentTo(controllerResponse);
        okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.Accepted,
            "Service should return 202 - Accepted when setting data");
    }
}