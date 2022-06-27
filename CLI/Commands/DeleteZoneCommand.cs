using System.CommandLine;
using CLI.Broker;
using CLI.Extensions;
using CLI.Models.DTOs;

namespace CLI.Commands;

public class DeleteZoneCommand : Command
{
    private readonly IAPIBroker _apiBroker;

    public DeleteZoneCommand(IAPIBroker apiBroker) : base("delete-zone", "Delete zone")
    {
        _apiBroker = apiBroker;
        var zoneName = new Argument<string>("name")
        {
            Name = "Name",
            Description = "The FQDN of the zone"
        };


        AddArgument(zoneName);

        this.SetHandler(ExecuteCommand, zoneName);
    }


    public async Task ExecuteCommand(string zoneName)
    {
        Console.WriteLine($"Trying to delete... {zoneName}");
        var tryFetchZone = await _apiBroker.GetZoneInfoAsync(zoneName);


        var zoneInfo = await tryFetchZone.ProcessHttpResponseAsync<Zone>("zone info");
        if (zoneInfo == null)
        {
            Console.WriteLine("Could not get zone info from API");
            return;
        }

        var tryDeleteZoneInfoAsync = await _apiBroker.DeleteZoneAsync(zoneInfo);


        var deleteInfo = await tryDeleteZoneInfoAsync.ProcessHttpResponseAsync<OperationResult>("delete zone");
        if (deleteInfo == null)
        {
            Console.WriteLine("Could delete zone");
            return;
        }

        Console.WriteLine($"Successfully deleted zone - {deleteInfo.Id}");
    }
}