using CLI.Models.DTOs;

namespace CLI.Broker;

public partial interface IAPIBroker
{
    Task<HttpResponseMessage> GetRecordByIDAsync(string documentID);

    Task<HttpResponseMessage> GetRecordAsync(string queryName, string type);

    Task<HttpResponseMessage> ListRecordAsync(uint zoneId);

    Task<HttpResponseMessage> SetRecordAsync(Record record);

    Task<HttpResponseMessage> DeleteRecordAsync(Record record);
}