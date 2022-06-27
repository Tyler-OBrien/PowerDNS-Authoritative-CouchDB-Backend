using System.Net.Http.Json;
using CLI.Extensions;
using CLI.Models.DTOs;

namespace CLI.Broker;

public partial class APIBroker
{
    public const string RecordsDB = "/v1/dnsapi/Record";

    public async Task<HttpResponseMessage> GetRecordAsync(string queryName, string type)
    {
        return await _httpClient.GetAsync($"{RecordsDB}/{queryName}/{type}");
    }

    public async Task<HttpResponseMessage> GetRecordByIDAsync(string recordId)
    {
        return await _httpClient.GetAsync($"{RecordsDB}/ID/{recordId}");
    }

    public async Task<HttpResponseMessage> ListRecordAsync(uint zoneId)
    {
        // This limit really shouldn't be hard-coded
        return await _httpClient.GetAsync($"{RecordsDB}/{zoneId}?limit=1000");
    }

    public async Task<HttpResponseMessage> SetRecordAsync(Record record)
    {
        return await _httpClient.PostAsJsonAsync($"{RecordsDB}/", record);
    }

    public async Task<HttpResponseMessage> DeleteRecordAsync(Record record)
    {
        return await _httpClient.DeleteAsJsonAsync($"{RecordsDB}", record);
    }
}