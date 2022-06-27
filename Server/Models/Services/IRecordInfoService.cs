using PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.Services;

public interface IRecordInfoService
{
    Task<List<Record>?> GetRecordAsync(string queryName, string type);

    Task<Record?> GetRecordByIdAsync(string recordId);

    Task<List<Record>?> ListRecordAsync(string queryName);


    Task<List<Record>?> ListRecordByZoneIdAsync(uint zoneID);

    Task<IOperationResult> SetRecordAsync(Record record);

    Task<IOperationResult> DeleteRecordAsync(Record record);
}