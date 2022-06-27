using System.Text.Json.Serialization;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.PowerDNS_Responses.Lookup;

public class LookupResponse : IDnsResultResponse<List<PdnsRecord>?>
{
    public LookupResponse()
    {
    }

    public LookupResponse(List<Record>? records)
    {
        if (records != null)
            Result = records.Select(info => new PdnsRecord(info)).ToList();
        else
            Result = new List<PdnsRecord>();
    }

    [JsonPropertyName("result")] public List<PdnsRecord>? Result { get; set; }
}