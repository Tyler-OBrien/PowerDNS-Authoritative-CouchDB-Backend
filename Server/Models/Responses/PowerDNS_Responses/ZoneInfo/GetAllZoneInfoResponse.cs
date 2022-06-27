using System.Text.Json.Serialization;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.PowerDNS_Responses.ZoneInfo;

public class GetAllZoneInfoResponse : IDnsResultResponse<List<PdnsZoneInfo>?>
{
    public GetAllZoneInfoResponse()
    {
    }

    public GetAllZoneInfoResponse(List<Zone>? zoneInfo)
    {
        if (zoneInfo != null)
            Result = zoneInfo.Select(info => new PdnsZoneInfo(info)).ToList();
        else
            Result = new List<PdnsZoneInfo>();
    }

    [JsonPropertyName("result")] public List<PdnsZoneInfo>? Result { get; set; }
}