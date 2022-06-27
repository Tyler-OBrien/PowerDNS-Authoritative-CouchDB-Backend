using System.Text.Json.Serialization;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.PowerDNS_Responses.ZoneInfo;

public class GetZoneInfoResponse : IDnsResultResponse<PdnsZoneInfo?>
{
    public GetZoneInfoResponse()
    {
    }

    public GetZoneInfoResponse(Zone? zoneInfo)
    {
        if (zoneInfo != null)
            Result = new PdnsZoneInfo(zoneInfo);
        else
            Result = new PdnsZoneInfo();
    }

    [JsonPropertyName("result")] public PdnsZoneInfo? Result { get; set; }
}