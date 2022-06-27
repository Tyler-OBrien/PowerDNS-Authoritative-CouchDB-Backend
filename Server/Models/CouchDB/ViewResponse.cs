using System.Text.Json.Serialization;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;

public class ViewResponse<T>
{
    [JsonPropertyName("offset")] public int Offset { get; set; }


    [JsonPropertyName("rows")] public List<ViewRowResponse<T>> Rows { get; set; }

    [JsonPropertyName("total_rows")] public int TotalRows { get; set; }
}

public class ViewRowResponse<T>
{
    [JsonPropertyName("id")] public string ID { get; set; }

    [JsonPropertyName("key")] public string Key { get; set; }

    [JsonPropertyName("doc")] public T Document { get; set; }
}