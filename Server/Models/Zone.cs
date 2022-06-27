using System.Text.Json.Serialization;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models;

public class Zone
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("_id")]
    public string? ID { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("_rev")]
    public string? Revision { get; set; }

    [JsonPropertyName("zoneId")] public uint ZoneId { get; set; }


    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("notified_serial")] public int? NotifiedSerial { get; set; }

    [JsonPropertyName("last_check")] public int? LastCheck { get; set; }

    [JsonPropertyName("masters")] public string[]? Masters { get; set; }

    public override string ToString()
    {
        return
            $"ID: {ID}, ZoneId: {ZoneId}, Type: {Type}, Notified Serial: {NotifiedSerial}, Last Check {LastCheck}, Masters : {string.Join(", ", Masters ?? Array.Empty<string>())}";
    }
}