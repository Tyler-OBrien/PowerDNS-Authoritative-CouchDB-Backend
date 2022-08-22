using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.CouchDB;
using Sentry;
using Serilog;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Extensions.HTTPClient;

public static class HTTPClientExtensions
{
    // We need case-insensitivity due to the differences between CouchDB and .NET's JSON Parsing
    public static readonly JsonSerializerOptions JsonSerializerOptions =
        new() { AllowTrailingCommas = true, PropertyNamingPolicy = null, PropertyNameCaseInsensitive = true };


    public static async Task HandleExceptionAsync(this HttpRequestException exception,
        [CallerMemberName] string context = "")
    {
        var CouchDBException =
            new CouchDBException($"Unexpected non-success status code - {exception.StatusCode}", exception);
        Log.Error(CouchDBException, $"Error in {context}");
        // If Sentry isn't enabled, this will do nothing
        SentrySdk.CaptureException(CouchDBException);
    }


    public static async Task<CouchDbOperationResult> GetCouchDBOperationResult(
        this HttpResponseMessage response)
    {
        // We can assume this is only going to be called with success status codes...
        var tryGetContent = await response.Content.ReadAsStringAsync();
        // TODO:
        // VALIDATE THIS!!!
        return JsonSerializer.Deserialize<CouchDbOperationResult>(tryGetContent);
    }


    public static async Task<T?> GetFromJsonAsyncSupportNull<T>(this HttpClient client, string? requestUri, CancellationToken token)
        where T : class
    {
        var response = await client.GetAsync(requestUri, token);
        response.ThrowForServerSideErrors();
        if (response.IsSuccessStatusCode)
        {
            var rawString = await response.Content.ReadAsStringAsync(token);


            if (string.IsNullOrWhiteSpace(rawString) == false)
            {
                var output = JsonSerializer.Deserialize<T>(rawString, JsonSerializerOptions);

                return output;
            }
        }

        return null;
    }

    public static async Task<List<T>?> CouchDBViewGetFromJsonAsyncSupportNull<T>(this HttpClient client,
        string? requestUri, CancellationToken token)
        where T : class
    {
        var response = await client.GetAsync(requestUri);
        response.ThrowForServerSideErrors();
        if (response.IsSuccessStatusCode)
        {
            var rawString = await response.Content.ReadAsStringAsync(token);


            if (string.IsNullOrWhiteSpace(rawString) == false)
            {
                var viewResponse = JsonSerializer.Deserialize<ViewResponse<T>>(rawString, JsonSerializerOptions);

                if (viewResponse != null) return viewResponse.Rows.Select(row => row.Document).ToList();
            }
        }

        return null;
    }


    public static async Task<TValue?> PostAsJsonGetJsonAsync<TInput, TValue>(this HttpClient client, string? requestUri,
        TInput input)
        where TValue : class
    {
        var response = await client.PostAsJsonAsync(requestUri, input);
        response.ThrowForServerSideErrors();
        if (response.IsSuccessStatusCode)
        {
            var rawString = await response.Content.ReadAsStringAsync();


            if (string.IsNullOrWhiteSpace(rawString) == false)
            {
                var output = JsonSerializer.Deserialize<TValue>(rawString, JsonSerializerOptions);

                return output;
            }
        }

        return null;
    }

    public static async Task<List<TValue>?> CouchDBFindPostAsJsonGetJsonAsync<TInput, TValue>(this HttpClient client,
        string? requestUri, TInput input, CancellationToken token = default)
        where TValue : class
    {
        var response = await client.PostAsJsonAsync(requestUri, input, token);
        response.ThrowForServerSideErrors();
        if (response.IsSuccessStatusCode)
        {
            var rawString = await response.Content.ReadAsStringAsync(token);


            if (string.IsNullOrWhiteSpace(rawString) == false)
            {
                var output = JsonSerializer.Deserialize<FindQueryResponse<TValue>>(rawString, JsonSerializerOptions);

                if (output != null) return output.Docs;
            }
        }

        return null;
    }

    public static void ThrowForServerSideErrors(this HttpResponseMessage msg)
    {
        if (msg.StatusCode != HttpStatusCode.NotFound && msg.StatusCode != HttpStatusCode.Conflict)
            msg.EnsureSuccessStatusCode();
    }
}