using System.Text.Json.Serialization;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;

public interface IOperationResult
{
    [JsonPropertyName("success")] public bool Success { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; }

    [JsonPropertyName("code")] public int Code { get; set; }
}