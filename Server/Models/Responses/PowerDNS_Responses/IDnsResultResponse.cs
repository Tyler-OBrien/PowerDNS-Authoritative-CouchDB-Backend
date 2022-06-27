using System.Text.Json.Serialization;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.PowerDNS_Responses;

// Generic Result Responses
public interface IDnsResultResponse<T> : IDnsResponse
{
    [JsonPropertyName("result")] public T? Result { get; set; }
}