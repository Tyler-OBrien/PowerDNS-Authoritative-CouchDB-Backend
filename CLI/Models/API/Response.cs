using System.Text.Json.Serialization;

namespace CLI.Models.API;

public class Response<T> : IResponse
{
    public Response()
    {
    }

    public Response(T data)
    {
        Data = data;
    }

    [JsonPropertyName("data")] public T? Data { get; set; }

    [JsonPropertyName("error")] public GenericError? Error { get; set; }
}