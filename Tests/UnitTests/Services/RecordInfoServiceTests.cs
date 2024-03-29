﻿using System.Net;
using System.Text.Json;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using PowerDNS_Auth_CouchDB_Remote_Backend.Brokers;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Services;
using PowerDNS_Auth_CouchDB_Remote_Backend.Services;

namespace UnitTests.Services;

public class RecordInfoServiceTests
{
    private readonly IAPIBroker _apiBroker;
    private readonly Mock<IAPIBroker> _mockApiBroker;
    private readonly IGeoIPService _geoIPService;
    private readonly Mock<IGeoIPService> _mockGeoIPService;

    private readonly RecordInfoService _recordInfoService;

    public RecordInfoServiceTests()
    {
        _mockApiBroker = new Mock<IAPIBroker>(MockBehavior.Strict);
        _mockGeoIPService = new Mock<IGeoIPService>();


        _apiBroker = _mockApiBroker.Object;
        _geoIPService = _mockGeoIPService.Object;
        
        _recordInfoService = new RecordInfoService(_apiBroker, _geoIPService);
    }

    [Test]
    [AutoData]
    public async Task ListRecordAsync(string queryName, List<Record> records)
    {
        // Arrange
        _mockApiBroker.Setup(service => service.ListRecordAsync(queryName, It.IsAny<CancellationToken>())).ReturnsAsync(records);
        // Act
        var response = await _recordInfoService.ListRecordAsync(queryName, string.Empty);
        //Assert
        response.Should().NotBeNull("Response should not be null");
        response.Should().BeEquivalentTo(records);
    }


    [Test]
    [AutoData]
    public async Task ListRecordAsyncUnnormalized(List<Record> records)
    {
        // Data
        var queryName = "example.com.";
        var actualName = "example.com";
        // Arrange
        _mockApiBroker.Setup(service => service.ListRecordAsync(actualName, It.IsAny<CancellationToken>())).ReturnsAsync(records);
        // Mock GeoIP Responses
        _mockGeoIPService.Setup(service => service.ProcessGeoIp(It.IsAny<List<Record>>(), It.IsAny<string>()))
            .ReturnsAsync((List<Record> value, string remoteIP) => value);
        // Act
        var response = await _recordInfoService.ListRecordAsync(queryName, string.Empty);
        //Assert
        response.Should().NotBeNull("Response should not be null");
        response.Should().BeEquivalentTo(records);
    }

    [Test]
    [AutoData]
    public async Task GetRecordById(Record record, string recordId)
    {
        // Arrange
        _mockApiBroker.Setup(service => service.GetRecordByIdAsync(recordId, It.IsAny<CancellationToken>())).ReturnsAsync(record);
        // Act
        var response = await _recordInfoService.GetRecordByIdAsync(recordId);
        //Assert
        response.Should().NotBeNull("Response should not be null");
        response.Should().BeEquivalentTo(record);
    }

    [Test]
    [AutoData]
    public async Task ListRecordByZoneIdAsync(List<Record> records, uint zoneId)
    {
        // Arrange
        _mockApiBroker.Setup(service => service.ListRecordByZoneIdAsync(zoneId, It.IsAny<CancellationToken>())).ReturnsAsync(records);
        // Mock GeoIP Responses
        _mockGeoIPService.Setup(service => service.ProcessGeoIp(It.IsAny<List<Record>>(), It.IsAny<string>()))
            .ReturnsAsync((List<Record> value, string remoteIP) => value);
        // Act
        var response = await _recordInfoService.ListRecordByZoneIdAsync(zoneId);
        //Assert
        response.Should().NotBeNull("Response should not be null");
        response.Should().BeEquivalentTo(records);
    }

    [Test]
    [AutoData]
    public async Task GetRecordAsync(List<Record> records, string queryName, string type)
    {
        // Arrange
        _mockApiBroker.Setup(service => service.GetRecordAsync(queryName, type, It.IsAny<CancellationToken>())).ReturnsAsync(records);
        // Mock GeoIP Responses
        _mockGeoIPService.Setup(service => service.ProcessGeoIp(It.IsAny<List<Record>>(), It.IsAny<string>()))
            .ReturnsAsync((List<Record> value, string remoteIP) => value);
        // Act
        var response = await _recordInfoService.GetRecordAsync(queryName, type, string.Empty);
        // Assert
        response.Should().NotBeNull("Response should not be null");
        response.Should().BeEquivalentTo(records);
    }

    [Test]
    [AutoData]
    public async Task SetRecordAsync(Record record, CouchDbOperationResult result)
    {
        // Data
        var Response = new HttpResponseMessage(HttpStatusCode.OK)
            { Content = new StringContent(JsonSerializer.Serialize(result)) };
        // Arrange
        _mockApiBroker.Setup(service => service.SetRecordAsync(record, It.IsAny<CancellationToken>())).ReturnsAsync(Response);
        // Act
        var response = await _recordInfoService.SetRecordAsync(record);
        // Assert
        response.Should().NotBeNull("Response should not be null");
        response.Code.Should().Be((int)HttpStatusCode.OK);

        var genericResult = response as GenericOperationResult<CouchDbOperationResult>;

        genericResult.Should().NotBeNull("Should return GenericOperationResult");
        genericResult.Data.Should().BeEquivalentTo(result);
    }

    [Test]
    [AutoData]
    public async Task SetRecordAsyncConflict(Record record)
    {
        // Data
        var Response = new HttpResponseMessage(HttpStatusCode.Conflict);
        // Arrange
        _mockApiBroker.Setup(service => service.SetRecordAsync(record, It.IsAny<CancellationToken>())).ReturnsAsync(Response);
        // Act
        var response = await _recordInfoService.SetRecordAsync(record);
        // Assert
        response.Should().NotBeNull("Response should not be null");
        response.Code.Should().Be((int)HttpStatusCode.Conflict);

        var genericResult = response as GenericOperationErrorResult;

        genericResult.Should().NotBeNull("Should return GenericOperationErrorResult");
        genericResult.Success.Should().BeFalse("Should not indictate success");
    }

    [Test]
    [AutoData]
    public async Task SetRecordAsyncError(Record record)
    {
        // Data
        var Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        // Arrange
        _mockApiBroker.Setup(service => service.SetRecordAsync(record, It.IsAny<CancellationToken>())).ReturnsAsync(Response);
        // Act
        var response = _recordInfoService.SetRecordAsync(record);
        // Assert
        Assert.ThrowsAsync<HttpRequestException>(() => response);
    }


    [Test]
    [AutoData]
    public async Task DeleteRecordAsync(Record record, CouchDbOperationResult result)
    {
        // Data
        var Response = new HttpResponseMessage(HttpStatusCode.OK)
            { Content = new StringContent(JsonSerializer.Serialize(result)) };
        // Arrange
        _mockApiBroker.Setup(service => service.DeleteRecordAsync(record, It.IsAny<CancellationToken>())).ReturnsAsync(Response);
        // Act
        var response = await _recordInfoService.DeleteRecordAsync(record);
        // Assert
        response.Should().NotBeNull("Response should not be null");
        response.Code.Should().Be((int)HttpStatusCode.OK);

        var genericResult = response as GenericOperationResult<CouchDbOperationResult>;

        genericResult.Should().NotBeNull("Should return GenericOperationResult");
        genericResult.Data.Should().BeEquivalentTo(result);
    }

    [Test]
    [AutoData]
    public async Task DeleteRecordAsyncNotFound(Record record)
    {
        // Data
        var Response = new HttpResponseMessage(HttpStatusCode.NotFound);
        // Arrange
        _mockApiBroker.Setup(service => service.DeleteRecordAsync(record, It.IsAny<CancellationToken>())).ReturnsAsync(Response);
        // Act
        var response = await _recordInfoService.DeleteRecordAsync(record);
        // Assert
        response.Should().NotBeNull("Response should not be null");
        response.Code.Should().Be((int)HttpStatusCode.NotFound);

        var genericResult = response as GenericOperationErrorResult;

        genericResult.Should().NotBeNull("Should return GenericOperationErrorResult");
        genericResult.Success.Should().BeFalse("Should not indictate success");
    }

    [Test]
    [AutoData]
    public async Task DeleteRecordAsyncError(Record record)
    {
        // Data
        var Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        // Arrange
        _mockApiBroker.Setup(service => service.DeleteRecordAsync(record, It.IsAny<CancellationToken>())).ReturnsAsync(Response);
        // Act
        var response = _recordInfoService.DeleteRecordAsync(record);
        // Assert
        Assert.ThrowsAsync<HttpRequestException>(() => response);
    }




}