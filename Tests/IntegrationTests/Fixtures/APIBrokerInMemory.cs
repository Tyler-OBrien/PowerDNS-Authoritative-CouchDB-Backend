using System.Net;
using System.Text.Json;
using PowerDNS_Auth_CouchDB_Remote_Backend.Brokers;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;

namespace IntegrationTests.Fixtures;

public class APIBrokerInMemory : IAPIBroker
{
    private readonly Dictionary<string, Record> _records;

    private readonly Dictionary<string, Zone> _zones;


    public APIBrokerInMemory()
    {
        _zones = new Dictionary<string, Zone>(StringComparer.OrdinalIgnoreCase);
        _records = new Dictionary<string, Record>(StringComparer.OrdinalIgnoreCase);
    }

    public async Task<Zone?> GetZoneInfoAsync(string zoneName, CancellationToken token = default)
    {
        return _zones.FirstOrDefault(zone => zone.Key.Equals(zoneName, StringComparison.OrdinalIgnoreCase)).Value;
    }

    public async Task<List<Zone>?> GetAllZoneInfoAsync(bool includeDisabled, CancellationToken token = default)
    {
        return _zones.Select(zone => zone.Value).ToList();
    }

    public async Task<HttpResponseMessage> SetZoneInfoAsync(Zone newZoneInfo, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(newZoneInfo.ID)) newZoneInfo.ID = Guid.NewGuid().ToString("N");

        if (_zones.ContainsKey(newZoneInfo.ID)) return new HttpResponseMessage(HttpStatusCode.Conflict);

        var result = new CouchDbOperationResult
            { Id = newZoneInfo.ID, Ok = true, Rev = $"1-{Guid.NewGuid().ToString("N")}" };
        _zones.Add(newZoneInfo.ID, newZoneInfo);
        return new HttpResponseMessage(HttpStatusCode.OK)
            { Content = new StringContent(JsonSerializer.Serialize(result)) };
    }

    public async Task<HttpResponseMessage> DeleteZoneAsync(Zone zone, CancellationToken token = default)
    {
        if (!_zones.ContainsKey(zone.ID)) return new HttpResponseMessage(HttpStatusCode.NotFound);

        _zones.Remove(zone.ID);
        var result = new CouchDbOperationResult { Id = zone.ID, Ok = true, Rev = $"1-{Guid.NewGuid().ToString("N")}" };
        return new HttpResponseMessage(HttpStatusCode.OK)
            { Content = new StringContent(JsonSerializer.Serialize(result)) };
    }

    public async Task<List<Record>?> GetRecordAsync(string queryName, string type, CancellationToken token = default)
    {
        return _records.Where(record =>
            record.Value.Name.Equals(queryName, StringComparison.OrdinalIgnoreCase) &&
            record.Value.Type.Equals(type, StringComparison.OrdinalIgnoreCase)).Select(record => record.Value).ToList();
    }

    public async Task<Record?> GetRecordByIdAsync(string recordId, CancellationToken token = default)
    {
        if (_records.TryGetValue(recordId, out var record))
            return record;
        return null;
    }

    public async Task<List<Record>?> ListRecordAsync(string queryName, CancellationToken token = default)
    {
        return _records.Where(record =>
                record.Value.Name.Equals(queryName, StringComparison.OrdinalIgnoreCase)).Select(record => record.Value)
            .ToList();
    }

    public async Task<List<Record>?> ListRecordByZoneIdAsync(uint zoneId, CancellationToken token = default)
    {
        return _records.Where(record =>
            record.Value.zoneId.Equals(zoneId)).Select(record => record.Value).ToList();
    }

    public async Task<HttpResponseMessage> SetRecordAsync(Record record, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(record.ID)) record.ID = Guid.NewGuid().ToString("N");
        if (_records.ContainsKey(record.ID)) return new HttpResponseMessage(HttpStatusCode.Conflict);
        _records.Add(record.ID, record);
        var result = new CouchDbOperationResult
            { Id = record.ID, Ok = true, Rev = $"1-{Guid.NewGuid().ToString("N")}" };
        return new HttpResponseMessage(HttpStatusCode.OK)
            { Content = new StringContent(JsonSerializer.Serialize(result)) };
    }

    public async Task<HttpResponseMessage> DeleteRecordAsync(Record record, CancellationToken token = default)
    {
        if (!_records.ContainsKey(record.ID)) return new HttpResponseMessage(HttpStatusCode.NotFound);

        _records.Remove(record.ID);

        var result = new CouchDbOperationResult
            { Id = record.ID, Ok = true, Rev = $"1-{Guid.NewGuid().ToString("N")}" };
        return new HttpResponseMessage(HttpStatusCode.OK)
            { Content = new StringContent(JsonSerializer.Serialize(result)) };
    }
}