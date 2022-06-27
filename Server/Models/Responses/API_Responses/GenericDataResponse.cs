using System.Net;
using System.Text.Json.Serialization;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.API_Responses;

public class GenericDataResponse : DataResponse<GenericData>
{
    //Deseralization
    private GenericDataResponse()
    {
    }

    public GenericDataResponse(HttpStatusCode code, string message)
    {
        Data = new GenericData(code, message);
    }

    public GenericDataResponse(int code, string message)
    {
        Data = new GenericData(code, message);
    }

    [JsonPropertyName("data")] public GenericData Data { get; set; }
}

public class GenericData
{
    private GenericData()
    {
    }

    internal GenericData(int code, string message)
    {
        Code = code;
        Message = message;
    }

    internal GenericData(HttpStatusCode code, string message)
    {
        Code = (int)code;
        Message = message;
    }

    [JsonPropertyName("code")]
    // HTTP Status Code
    public int Code { get; set; }

    [JsonPropertyName("Message")] public string Message { get; set; }
}