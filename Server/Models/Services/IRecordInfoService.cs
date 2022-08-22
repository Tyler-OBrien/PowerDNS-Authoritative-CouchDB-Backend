using PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.Services;

public interface IRecordInfoService
{
    Task<List<Record>?> GetRecordAsync(string queryName, string type, CancellationToken token = default);

    Task<Record?> GetRecordByIdAsync(string recordId, CancellationToken token = default);

    Task<List<Record>?> ListRecordAsync(string queryName, CancellationToken token = default);


    Task<List<Record>?> ListRecordByZoneIdAsync(uint zoneID, CancellationToken token = default);

    Task<IOperationResult> SetRecordAsync(Record record, CancellationToken token = default);

    Task<IOperationResult> DeleteRecordAsync(Record record, CancellationToken token = default);
}