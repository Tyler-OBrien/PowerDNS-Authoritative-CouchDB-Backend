using System.Net.Http.Headers;
using System.Text.Json;
using CLI.Models.API;

namespace CLI.Extensions;

public static class HTTPClientExtensions
{
    public static async Task<HttpResponseMessage> DeleteAsJsonAsync<TInput>(this HttpClient client, string? requestUri,
        TInput input)
    {
        var jsonInput = JsonSerializer.Serialize(input);
        var newRequest = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        newRequest.Content = new StringContent(jsonInput);
        newRequest.Method = HttpMethod.Delete;
        newRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var httpResponse = await client.SendAsync(newRequest);
        return httpResponse;
    }


    public static async Task<T?> ProcessHttpResponseAsync<T>(this HttpResponseMessage httpResponse, string assetName)
        where T : class
    {
        try
        {
            var rawString = await httpResponse.Content.ReadAsStringAsync();


            if (string.IsNullOrWhiteSpace(rawString))
            {
                Console.WriteLine(
                    $"Could not get response {assetName} from API, API returned nothing, Status Code: {httpResponse.StatusCode}");
                return null;
            }

            var response = JsonSerializer.Deserialize<Response<T>>(rawString);

            if (response == null)
            {
                Console.WriteLine($"Could not get response {assetName} from API");
                return null;
            }

            if (response.Error != null)
            {
                Console.WriteLine($"Error with {assetName}: {response.Error.Code} - {response.Error.Message}");
                return null;
            }

            if (response.Data == null)
            {
                Console.WriteLine($"Unknown error with {assetName}");
                return null;
            }

            return response.Data;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex);
            Console.WriteLine($"Unexpected HTTP Error: API Returned: {httpResponse?.StatusCode} - {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Console.WriteLine($"Unexpected Error: API Returned: {httpResponse?.StatusCode}");
        }

        return null;
    }
}