using System.Text.Json.Serialization;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.PowerDNS_Responses;

public class ResultResponseNone : IDnsResultResponse<bool>
{
    [JsonPropertyName("result")] public bool Result { get; set; } = false;
}