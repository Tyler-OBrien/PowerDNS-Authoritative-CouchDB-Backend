using System.Text.Json.Serialization;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;

public class FindQueryResponse<T>
{
    //https://docs.couchdb.org/en/3.2.0/api/database/find.html


    [JsonPropertyName("docs")] public List<T> Docs { get; set; }

    [JsonPropertyName("bookmark")] public string Bookmark { get; set; }

    [JsonPropertyName("warning")] public string ExecutionWarning { get; set; }
}