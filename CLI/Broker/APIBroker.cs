namespace CLI.Broker;

public partial class APIBroker : IAPIBroker
{
    private readonly HttpClient _httpClient;


    public APIBroker(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _httpClient.BaseAddress = new Uri("http://localhost:5112");
    }

    public void SetBaseAddress(string newBaseAddress)
    {
        _httpClient.BaseAddress = new Uri(newBaseAddress);
    }
}