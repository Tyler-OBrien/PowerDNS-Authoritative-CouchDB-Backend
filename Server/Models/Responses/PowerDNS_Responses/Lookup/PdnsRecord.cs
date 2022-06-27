using System.Text.Json.Serialization;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.PowerDNS_Responses.Lookup;

public class PdnsRecord
{
    public PdnsRecord()
    {
    }

    public PdnsRecord(string qType, string qName, string content, int tTL, uint zoneId, bool auth)
    {
        QType = qType;
        QName = qName;
        Content = content;
        TTL = tTL;
        zoneId = zoneId;
        Auth = auth;
    }

    public PdnsRecord(Record record)
    {
        QType = record.Type;
        QName = record.Name;
        Content = record.Content;
        TTL = record.TTL;
        zoneId = record.zoneId;
        Auth = record.Auth;
    }

    [JsonPropertyName("qtype")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

    public string QType { get; set; }

    [JsonPropertyName("qname")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

    public string QName { get; set; }

    [JsonPropertyName("content")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

    public string Content { get; set; }

    [JsonPropertyName("ttl")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int TTL { get; set; }

    [JsonPropertyName("zone_id")]
    [JsonIgnore]
    public uint zoneId { get; set; }


    [JsonPropertyName("auth")]
    [JsonIgnore]
    public bool Auth { get; set; }
}