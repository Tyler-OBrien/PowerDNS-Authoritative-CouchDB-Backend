using System.Text.Json.Serialization;

namespace CLI.Models.DTOs;

public class Record
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("_id")]
    public string? ID { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("_rev")]
    public string? Revision { get; set; }

    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("content")] public string Content { get; set; }

    [JsonPropertyName("ttl")] public int TTL { get; set; }

    [JsonPropertyName("zone_id")] public uint zoneId { get; set; }

    [JsonPropertyName("auth")] public bool Auth { get; set; }

    // Extra Flag, used for internal purposes, for example if this is a GEOIP or Latency-based record situation
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("flag")]
    public string? Flag { get; set; }

    [JsonPropertyName("disabled")] public bool Disabled { get; set; }

    // Note Prio is part of MX/SRV records directly now


    public override string ToString()
    {
        return $"{Name} {Type} {Content} {TTL} {Flag}, Zone ID: {zoneId}, disabled: {Disabled}";
    }
}