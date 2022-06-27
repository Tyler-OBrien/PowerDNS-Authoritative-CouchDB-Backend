using System.Text.Json.Serialization;

namespace CLI.Models.DTOs;

public class OperationResult
{
    [JsonPropertyName("ok")] public bool Ok { get; set; }

    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("rev")] public string Rev { get; set; }
}