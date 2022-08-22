using System.Net;
using System.Text.Json;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using PowerDNS_Auth_CouchDB_Remote_Backend.Brokers;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;
using PowerDNS_Auth_CouchDB_Remote_Backend.Services;

namespace UnitTests.Services;

public class ZoneInfoServiceTests
{
    private readonly IAPIBroker _apiBroker;
    private readonly Mock<IAPIBroker> _mockApiBroker;

    private readonly ZoneInfoService _zoneInfoService;

    public ZoneInfoServiceTests()
    {
        _mockApiBroker = new Mock<IAPIBroker>(MockBehavior.Strict);
        _apiBroker = _mockApiBroker.Object;
        _zoneInfoService = new ZoneInfoService(_apiBroker);
    }

    [Test]
    [AutoData]
    public async Task GetZoneInfoAsync(string zoneName, Zone zone)
    {
        // Arrange
        _mockApiBroker.Setup(service => service.GetZoneInfoAsync(zoneName, It.IsAny<CancellationToken>())).ReturnsAsync(zone);
        // Act
        var response = await _zoneInfoService.GetZoneInfoAsync(zoneName);
        //Assert
        response.Should().NotBeNull("Response should not be null");
        response.Should().BeEquivalentTo(zone);
    }

    [Test]
    [AutoData]
    public async Task GetAllZoneInfoAsync(List<Zone> zones)
    {
        // Data 
        var includeDisabled = true;
        // Arrange
        _mockApiBroker.Setup(service => service.GetAllZoneInfoAsync(includeDisabled, It.IsAny<CancellationToken>())).ReturnsAsync(zones);
        // Act
        var response = await _zoneInfoService.GetAllZoneInfoAsync(includeDisabled);
        //Assert
        response.Should().NotBeNull("Response should not be null");
        response.Should().BeEquivalentTo(zones);
    }

    [Test]
    [AutoData]
    public async Task SetZoneInfoAsync(Zone newZoneInfo, CouchDbOperationResult result)
    {
        // Data 
        var Response = new HttpResponseMessage(HttpStatusCode.OK)
            { Content = new StringContent(JsonSerializer.Serialize(result)) };

        // Arrange
        _mockApiBroker.Setup(service => service.SetZoneInfoAsync(newZoneInfo, It.IsAny<CancellationToken>())).ReturnsAsync(Response);
        // Act
        var response = await _zoneInfoService.SetZoneInfoAsync(newZoneInfo);
        //Assert
        response.Should().NotBeNull("Response should not be null");
        var genericResult = response as GenericOperationResult<CouchDbOperationResult>;
        genericResult.Should().NotBeNull("Should return GenericOperationResult");
        genericResult.Data.Should().BeEquivalentTo(result);
    }

    [Test]
    [AutoData]
    public async Task DeleteZoneInfoAsync(Zone newZoneInfo, CouchDbOperationResult result)
    {
        // Data 
        var Response = new HttpResponseMessage(HttpStatusCode.OK)
            { Content = new StringContent(JsonSerializer.Serialize(result)) };

        // Arrange
        _mockApiBroker.Setup(service => service.DeleteZoneAsync(newZoneInfo, It.IsAny<CancellationToken>())).ReturnsAsync(Response);
        // Act
        var response = await _zoneInfoService.DeleteZoneAsync(newZoneInfo);
        //Assert
        response.Should().NotBeNull("Response should not be null");
        var genericResult = response as GenericOperationResult<CouchDbOperationResult>;
        genericResult.Should().NotBeNull("Should return GenericOperationResult");
        genericResult.Data.Should().BeEquivalentTo(result);
    }


    [Test]
    [AutoData]
    public async Task SetZoneInfoAsyncThrow(Zone newZoneInfo)
    {
        // Data 
        var Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        // Arrange
        _mockApiBroker.Setup(service => service.SetZoneInfoAsync(newZoneInfo, It.IsAny<CancellationToken>())).ReturnsAsync(Response);
        // Act
        var response = _zoneInfoService.SetZoneInfoAsync(newZoneInfo);
        //Assert
        Assert.ThrowsAsync<HttpRequestException>(() => response);
    }


    [Test]
    [AutoData]
    public async Task DeleteZoneInfoAsyncThrow(Zone newZoneInfo)
    {
        // Data 
        var Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        // Arrange
        _mockApiBroker.Setup(service => service.DeleteZoneAsync(newZoneInfo, It.IsAny<CancellationToken>())).ReturnsAsync(Response);
        // Act
        var response = _zoneInfoService.DeleteZoneAsync(newZoneInfo);
        //Assert
        Assert.ThrowsAsync<HttpRequestException>(() => response);
    }

    [Test]
    [InlineAutoData(404)]
    [InlineAutoData(409)]
    public async Task DeleteZoneInfoAsyncConflict(HttpStatusCode code, Zone newZoneInfo)
    {
        // Data 

        var Response = new HttpResponseMessage(code);

        // Arrange
        _mockApiBroker.Setup(service => service.DeleteZoneAsync(newZoneInfo, It.IsAny<CancellationToken>())).ReturnsAsync(Response);
        // Act
        var response = await _zoneInfoService.DeleteZoneAsync(newZoneInfo);
        //Assert
        response.Should().NotBeNull("Response should not be null");
        var genericResult = response as GenericOperationErrorResult;
        genericResult.Should().NotBeNull("Should return GenericOperationErrorResult");
        genericResult.Code.Should().Be((int)code);
    }
}