using System.Text.Json.Serialization;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;

public class FindQuery
{
    public FindQuery()
    {
    }

    public FindQuery(Dictionary<string, dynamic> selector, List<string> fields)
    {
        Selector = selector;
        Fields = fields;
    }

    [JsonPropertyName("selector")] public Dictionary<string, dynamic> Selector { get; set; }

    [JsonPropertyName("fields")] public List<string> Fields { get; set; }
}