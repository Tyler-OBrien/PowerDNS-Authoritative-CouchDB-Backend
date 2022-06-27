using System.Text.Json.Serialization;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;

public class CouchDbOperationResult
{
    [JsonPropertyName("ok")] public bool Ok { get; set; }

    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("rev")] public string Rev { get; set; }
}