using PowerDNS_Auth_CouchDB_Remote_Backend.Models;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Brokers;

public partial interface IAPIBroker
{
    Task<List<Record>?> GetRecordAsync(string queryName, string type, CancellationToken token);
    Task<Record?> GetRecordByIdAsync(string recordId, CancellationToken token);

    Task<List<Record>?> ListRecordAsync(string queryName, CancellationToken token);


    Task<List<Record>?> ListRecordByZoneIdAsync(uint zoneId, CancellationToken token);

    Task<HttpResponseMessage> SetRecordAsync(Record record, CancellationToken token);

    Task<HttpResponseMessage> DeleteRecordAsync(Record record, CancellationToken token);
}