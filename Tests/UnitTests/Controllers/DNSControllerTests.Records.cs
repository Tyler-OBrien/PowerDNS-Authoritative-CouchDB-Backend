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
    public async Task GetRecordTestANY(List<Record> records, string zoneName)
    {
        // Data


        // Arrange
        _mockRecordInfoService.Setup(service => service.ListRecordAsync(zoneName, It.IsAny<CancellationToken>())).ReturnsAsync(records);

        // Act
        var response = await _controller.GetRecord(zoneName, "ANY");


        //Assert
        var rightResponse = new DataResponse<List<Record>>(records);


        var okObjectResult = response.Result as OkObjectResult;

        okObjectResult.Should().NotBeNull("Ok Object Result should be returned");


        okObjectResult.Value.Should().BeEquivalentTo(rightResponse);
        okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK,
            "Service should return 200 - OK when returning data");
    }

    [Test]
    [AutoData]
    public async Task GetRecordTestType(List<Record> records, string zoneName, string type)
    {
        // Data


        // Arrange
        _mockRecordInfoService.Setup(service => service.GetRecordAsync(zoneName, type, It.IsAny<CancellationToken>())).ReturnsAsync(records);

        // Act
        var response = await _controller.GetRecord(zoneName, type);


        //Assert
        var rightResponse = new DataResponse<List<Record>>(records);

        var okObjectResult = response.Result as OkObjectResult;


        okObjectResult.Should().NotBeNull("Ok Object Result should be returned");

        okObjectResult.Value.Should().BeEquivalentTo(rightResponse);
        okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK,
            "Service should return 200 - OK when returning data");
    }

    [Test]
    [AutoData]
    public async Task GetRecordById(Record record, string recordId)
    {
        // Data


        // Arrange
        _mockRecordInfoService.Setup(service => service.GetRecordByIdAsync(recordId, It.IsAny<CancellationToken>())).ReturnsAsync(record);

        // Act
        var response = await _controller.GetRecordById(recordId);


        //Assert
        var rightResponse = new DataResponse<Record>(record);


        var okObjectResult = response.Result as OkObjectResult;


        okObjectResult.Should().NotBeNull("Ok Object Result should be returned");

        okObjectResult.Value.Should().BeEquivalentTo(rightResponse);
        okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK,
            "Service should return 200 - OK when returning data");
    }

    [Test]
    [AutoData]
    public async Task GetRecordByIdNotFound(string recordId)
    {
        // Data


        // Arrange
        Record? newRecord = null;
        _mockRecordInfoService.Setup(service => service.GetRecordByIdAsync(recordId, It.IsAny<CancellationToken>())).ReturnsAsync(newRecord);

        // Act
        var response = await _controller.GetRecordById(recordId);


        //Assert

        var notFoundObject = response.Result as NotFoundObjectResult;

        notFoundObject.Value.Should()
            .BeOfType(typeof(GenericDataResponse), "Should get generic 404");
        notFoundObject.StatusCode.Should().Be((int)HttpStatusCode.NotFound,
            "If null is returned by the service, the controller should return notfound");
    }

    [Test]
    [AutoData]
    public async Task ListRecords(uint zoneID, List<Record> records)
    {
        // Data


        // Arrange
        _mockRecordInfoService.Setup(service => service.ListRecordByZoneIdAsync(zoneID, It.IsAny<CancellationToken>())).ReturnsAsync(records);

        // Act
        var response = await _controller.ListRecords(zoneID);


        //Assert
        var rightResponse = new DataResponse<List<Record>>(records);

        var okObjectResult = response.Result as OkObjectResult;


        okObjectResult.Should().NotBeNull("Ok Object Result should be returned");

        okObjectResult.Value.Should().BeEquivalentTo(rightResponse);
        okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK,
            "Service should return 200 - OK when returning data");
    }

    [Test]
    [AutoData]
    public async Task NewRecord(Record newRecord, CouchDbOperationResult result)
    {
        // Data
        var apiresponse = new GenericOperationResult<CouchDbOperationResult>(true, "", HttpStatusCode.OK, result);
        var controllerResponse = new DataResponse<CouchDbOperationResult>(result);

        // Arrange
        _mockRecordInfoService.Setup(service => service.SetRecordAsync(newRecord, It.IsAny<CancellationToken>())).ReturnsAsync(apiresponse);

        // Act
        var response = await _controller.NewRecord(newRecord);


        //Assert

        var okObjectResult = response.Result as AcceptedResult;


        okObjectResult.Should().NotBeNull("Ok Object Result should be returned");


        okObjectResult.Value.Should().BeEquivalentTo(controllerResponse);
        okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.Accepted,
            "Service should return 202 - Accepted when setting data");
    }

    [Test]
    [AutoData]
    public async Task NewRecordFail(Record newRecord, CouchDbOperationResult result, string msg)
    {
        // Data
        var code = HttpStatusCode.NotFound;
        var apiresponse = new GenericOperationResult<CouchDbOperationResult>(false, msg, code, result);

        // Arrange
        _mockRecordInfoService.Setup(service => service.SetRecordAsync(newRecord, It.IsAny<CancellationToken>())).ReturnsAsync(apiresponse);

        // Act
        var response = await _controller.NewRecord(newRecord);


        //Assert

        var okObjectResult = response.Result as ObjectResult;


        okObjectResult.Should().NotBeNull("Ok Object Result should be returned");


        var errorResponse = okObjectResult.Value as ErrorResponseDetails<IOperationResult>;

        errorResponse.Should().NotBeNull("We should return a proper ErrorResponseDetails");

        errorResponse.Error.Code.Should().Be((int)code, "Should return not found");
        errorResponse.Error.Details.Message.Should().Be(msg, "Message returned by service should be passed through");
        okObjectResult.StatusCode.Should().Be((int)code,
            "Service should return correct status code when returning data");
    }

    [Test]
    [AutoData]
    public async Task DeleteRecord(Record newRecord, CouchDbOperationResult result)
    {
        // Data
        var apiresponse = new GenericOperationResult<CouchDbOperationResult>(true, "", HttpStatusCode.OK, result);
        var controllerResponse = new DataResponse<CouchDbOperationResult>(result);

        // Arrange
        _mockRecordInfoService.Setup(service => service.DeleteRecordAsync(newRecord, It.IsAny<CancellationToken>())).ReturnsAsync(apiresponse);

        // Act
        var response = await _controller.DeleteRecord(newRecord);


        //Assert

        var okObjectResult = response.Result as AcceptedResult;


        okObjectResult.Should().NotBeNull("Ok Object Result should be returned");


        okObjectResult.Value.Should().BeEquivalentTo(controllerResponse);
        okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.Accepted,
            "Service should return 202 - Accepted when setting data");
    }

    [Test]
    [AutoData]
    public async Task DeleteRecordFail(Record newRecord, CouchDbOperationResult result, string msg)
    {
        // Data
        var code = HttpStatusCode.NotFound;

        var apiresponse = new GenericOperationResult<CouchDbOperationResult>(false, msg, code, result);

        // Arrange
        _mockRecordInfoService.Setup(service => service.DeleteRecordAsync(newRecord, It.IsAny<CancellationToken>())).ReturnsAsync(apiresponse);

        // Act
        var response = await _controller.DeleteRecord(newRecord);


        //Assert

        var okObjectResult = response.Result as ObjectResult;


        okObjectResult.Should().NotBeNull("Ok Object Result should be returned");

        var errorResponse = okObjectResult.Value as ErrorResponseDetails<IOperationResult>;

        errorResponse.Should().NotBeNull("We should return a proper ErrorResponseDetails");

        errorResponse.Error.Code.Should().Be((int)code, "Should return not found");
        errorResponse.Error.Details.Message.Should().Be(msg, "Message returned by service should be passed through");
        okObjectResult.StatusCode.Should().Be((int)code,
            "Service should return 200 - OK when returning data");
    }
}