using System.Text.Json.Serialization;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.PowerDNS_Responses.ZoneInfo;

// This is the format PowerDNS expects for responses about Zones/Domains
public class PdnsZoneInfo
{
    public PdnsZoneInfo()
    {
    }

    public PdnsZoneInfo(Zone zoneInfo)
    {
        ID = zoneInfo.ZoneId;
        Kind = zoneInfo.Type;
        Serial = zoneInfo.NotifiedSerial ?? -1;
        // We know it won't be null.
        Zone = zoneInfo.ID;
    }

    public PdnsZoneInfo(uint id, string zone, string kind, int serial)
    {
        ID = id;
        Zone = zone;
        Kind = kind;
        Serial = serial;
    }

    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]

    public uint ID { get; set; }

    [JsonPropertyName("zone")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

    public string Zone { get; set; }

    [JsonPropertyName("kind")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

    public string Kind { get; set; }

    [JsonPropertyName("serial")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Serial { get; set; }
}