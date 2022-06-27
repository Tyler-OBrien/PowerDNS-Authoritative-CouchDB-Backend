using System.Net;
using System.Text.Json.Serialization;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;

public class GenericOperationErrorResult : IOperationResult
{
    private GenericOperationErrorResult()
    {
    }

    public GenericOperationErrorResult(bool success, string message, HttpStatusCode code)
    {
        Success = success;
        Message = message;
        Code = (int)code;
    }

    public GenericOperationErrorResult(bool success, string message, int code)
    {
        Success = success;
        Message = message;
        Code = code;
    }

    public bool Data
    {
        get => Success;

        set => Success = value;
    }

    [JsonPropertyName("success")] public bool Success { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; }

    [JsonPropertyName("code")] public int Code { get; set; }
}

public class GenericOperationResult<T> : IOperationResult
{
    private GenericOperationResult()
    {
    }

    public GenericOperationResult(bool success, string message, HttpStatusCode code, T data)
    {
        Success = success;
        Message = message;
        Code = (int)code;
        Data = data;
    }

    public GenericOperationResult(bool success, string message, int code, T data)
    {
        Success = success;
        Message = message;
        Code = code;
        Data = data;
    }

    [JsonPropertyName("data")] public T Data { get; set; }

    [JsonPropertyName("success")] public bool Success { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; }

    [JsonPropertyName("code")] public int Code { get; set; }
}