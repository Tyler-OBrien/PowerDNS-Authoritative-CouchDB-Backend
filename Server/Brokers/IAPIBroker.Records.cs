using PowerDNS_Auth_CouchDB_Remote_Backend.Models;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Brokers;

public partial interface IAPIBroker
{
    Task<List<Record>?> GetRecordAsync(string queryName, string type);
    Task<Record?> GetRecordByIdAsync(string recordId);

    Task<List<Record>?> ListRecordAsync(string queryName);


    Task<List<Record>?> ListRecordByZoneIdAsync(uint zoneId);

    Task<HttpResponseMessage> SetRecordAsync(Record record);

    Task<HttpResponseMessage> DeleteRecordAsync(Record record);
}