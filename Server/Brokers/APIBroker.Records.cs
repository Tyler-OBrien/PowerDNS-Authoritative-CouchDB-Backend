using PowerDNS_Auth_CouchDB_Remote_Backend.Extensions.HTTPClient;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Brokers;

public partial class APIBroker : IAPIBroker
{
    public readonly string RecordsDB;

    public async Task<List<Record>?> GetRecordAsync(string queryName, string type, CancellationToken token)
    {
        var selector = new Dictionary<string, dynamic>
        {
            { "type", type },
            { "name", queryName }
        };
        // Could use Reflection for this, but not needed yet.
        var fields = new List<string>
            { "_id", "type", "name", "content", "ttl", "zone_id", "auth", "flag", "disabled" };
        var newFindQuery = new FindQuery(selector, fields);
        return await _httpClient.CouchDBFindPostAsJsonGetJsonAsync<FindQuery, Record>($"/{RecordsDB}/_find",
            newFindQuery, token);
    }

    public async Task<Record?> GetRecordByIdAsync(string documentId, CancellationToken token)
    {
        return await _httpClient.GetFromJsonAsyncSupportNull<Record>($"/{RecordsDB}/{documentId}", token);
    }


    public async Task<List<Record>?> ListRecordAsync(string queryName, CancellationToken token)
    {
        var selector = new Dictionary<string, dynamic>
        {
            { "name", queryName }
        };
        var fields = new List<string>
            { "_id", "type", "name", "content", "ttl", "zone_id", "auth", "flag", "disabled" };
        var newFindQuery = new FindQuery(selector, fields);
        return await _httpClient.CouchDBFindPostAsJsonGetJsonAsync<FindQuery, Record>($"/{RecordsDB}/_find",
            newFindQuery);
    }

    public async Task<List<Record>?> ListRecordByZoneIdAsync(uint zoneId, CancellationToken token)
    {
        var selector = new Dictionary<string, dynamic>
        {
            { "zone_id", zoneId }
        };
        var fields = new List<string>
            { "_id", "type", "name", "content", "ttl", "zone_id", "auth", "flag", "disabled" };
        var newFindQuery = new FindQuery(selector, fields);
        return await _httpClient.CouchDBFindPostAsJsonGetJsonAsync<FindQuery, Record>($"/{RecordsDB}/_find",
            newFindQuery);
    }

    public async Task<HttpResponseMessage> SetRecordAsync(Record record, CancellationToken token)
    {
        return await _httpClient.PostAsJsonAsync($"/{RecordsDB}/", record, token);
    }

    public async Task<HttpResponseMessage> DeleteRecordAsync(Record record, CancellationToken token)
    {
        return await _httpClient.DeleteAsync($"/{RecordsDB}/{Uri.EscapeDataString(record.ID)}?rev={record.Revision}", token);
    }
}