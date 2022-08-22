using System.Net;
using Microsoft.AspNetCore.Mvc;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.API_Responses;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Controllers;

public partial class DNSController
{
    [HttpGet("Record/{qName}/{qType}")]
    public async Task<ActionResult<IResponse>> GetRecord(string qName, string qType, CancellationToken token = default)
    {
        List<Record>? foundRecords;
        if (qType.Equals("ANY", StringComparison.OrdinalIgnoreCase))
            foundRecords = await _recordInfoService.ListRecordAsync(qName, token);
        else
            foundRecords = await _recordInfoService.GetRecordAsync(qName, qType, token);

        if (foundRecords != null) return Ok(new DataResponse<List<Record>>(foundRecords));
        return Ok(new DataResponse<List<Record>>(new List<Record>()));
    }

    [HttpGet("Record/ID/{recordId}")]
    public async Task<ActionResult<IResponse>> GetRecordById(string recordId, CancellationToken token = default)
    {
        var foundRecords = await _recordInfoService.GetRecordByIdAsync(recordId, token);
        if (foundRecords != null) return Ok(new DataResponse<Record>(foundRecords));
        return NotFound(new GenericDataResponse(HttpStatusCode.NotFound, $"Could not find record with ID {recordId}"));
    }


    [HttpGet("Record/{zoneId}")]
    public async Task<ActionResult<IResponse>> ListRecords(uint zoneId, CancellationToken token = default)
    {
        var foundRecords = await _recordInfoService.ListRecordByZoneIdAsync(zoneId, token);
        if (foundRecords != null) return Ok(new DataResponse<List<Record>>(foundRecords));
        return Ok(new DataResponse<List<Record>>(new List<Record>()));
    }


    [HttpPost("Record")]
    public async Task<ActionResult<IResponse>> NewRecord([FromBody] Record record, CancellationToken token = default)
    {
        var tryCreate = await _recordInfoService.SetRecordAsync(record, token);
        // We return accepted because one couchdb node accepting doesn't mean the others will...
        if (tryCreate.Success && tryCreate is GenericOperationResult<CouchDbOperationResult> tryCreateResult)
            return Accepted(new DataResponse<CouchDbOperationResult>(tryCreateResult.Data));
        return StatusCode(tryCreate.Code,
            new ErrorResponseDetails<IOperationResult>(tryCreate.Code, $"Failed to create record - {tryCreate.Message}",
                "record_creation_error", tryCreate));
    }

    [HttpDelete("Record")]
    public async Task<ActionResult<IResponse>> DeleteRecord(Record record, CancellationToken token = default)
    {
        var tryDelete = await _recordInfoService.DeleteRecordAsync(record, token);
        if (tryDelete.Success && tryDelete is GenericOperationResult<CouchDbOperationResult> tryDeleteResult)
            return Accepted(new DataResponse<CouchDbOperationResult>(tryDeleteResult.Data));
        return StatusCode(tryDelete.Code,
            new ErrorResponseDetails<IOperationResult>(tryDelete.Code, $"Failed to delete record - {tryDelete.Message}",
                "record_deletion_error", tryDelete));
    }
}