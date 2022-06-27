using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Configuration;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Brokers;

public partial class APIBroker : IAPIBroker
{
    private readonly ApplicationConfig _applicationConfig;
    private readonly HttpClient _httpClient;


    public APIBroker(HttpClient httpClient, IOptions<ApplicationConfig> applicationConfig)
    {
        _applicationConfig = applicationConfig.Value;
        _httpClient = httpClient;

        _httpClient.BaseAddress = new Uri(_applicationConfig.CouchDB_URL);

        var authBase64String =
            Convert.ToBase64String(
                Encoding.ASCII.GetBytes(
                    $"{_applicationConfig.CouchDB_Username}:{_applicationConfig.CouchDB_Password}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authBase64String);

        RecordsDB = _applicationConfig.CouchDB_Records_Database;
        ZonesDB = _applicationConfig.CouchDB_Zones_Database;
    }
}