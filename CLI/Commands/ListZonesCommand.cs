using System.CommandLine;
using CLI.Broker;
using CLI.Extensions;
using CLI.Models.DTOs;

namespace CLI.Commands;

public class ListZoneCommand : Command
{
    private readonly IAPIBroker _apiBroker;

    public ListZoneCommand(IAPIBroker apiBroker) : base("list-zones", "List zones")
    {
        _apiBroker = apiBroker;
        // Not possible unless set directly in db
        var includeDisabled = new Option<bool>("--disabled")
        {
            Name = "--includeDisabled",
            Description = "Include Disabled Zones",
            IsRequired = false
        };


        AddOption(includeDisabled);

        this.SetHandler(ExecuteCommand, includeDisabled);
    }


    public async Task ExecuteCommand(bool includeDisabled)
    {
        Console.WriteLine("Listing zones...");
        var tryFetchZones = await _apiBroker.GetAllZoneInfoAsync(includeDisabled);


        var zones = await tryFetchZones.ProcessHttpResponseAsync<List<Zone>>("zones");
        if (zones == null)
        {
            Console.WriteLine("Could not get zones from API");
            return;
        }

        if (zones.Any() == false)
        {
            Console.WriteLine("Found no zones");
            return;
        }

        Console.WriteLine("ID\t\tPDNS ID\t\tType\t\tLast Check\t\tNotified Serial\t\tMasters");
        foreach (var zone in zones)
            Console.WriteLine(
                $"{zone.ID}\t{zone.ZoneId}\t{zone.Type}\t{zone.LastCheck}\t{zone.NotifiedSerial}\t{zone.Masters}");
    }
}