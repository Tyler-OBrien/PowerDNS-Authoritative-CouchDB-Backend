using CLI.Models.DTOs;

namespace CLI.Broker;

public partial interface IAPIBroker
{
    Task<HttpResponseMessage> GetZoneInfoAsync(string zoneName);

    Task<HttpResponseMessage> GetAllZoneInfoAsync(bool includeDisabled);

    Task<HttpResponseMessage> SetZoneInfoAsync(Zone newZoneInfo);

    Task<HttpResponseMessage> DeleteZoneAsync(Zone zone);
}