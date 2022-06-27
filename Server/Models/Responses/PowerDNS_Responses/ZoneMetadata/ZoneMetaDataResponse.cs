namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.PowerDNS_Responses.ZoneMetadata;

public class ZoneMetaDataResponse : IDnsResultResponse<List<string>>
{
    public List<string> Result { get; set; } = new();
}