using System.CommandLine;
using CLI.Broker;
using CLI.Extensions;
using CLI.Models.DTOs;

namespace CLI.Commands;

public class CreateZoneCommand : Command
{
    private readonly IAPIBroker _apiBroker;

    public CreateZoneCommand(IAPIBroker apiBroker) : base("create-zone", "Create a zone")
    {
        _apiBroker = apiBroker;
        var zoneName = new Argument<string>("name")
        {
            Name = "Name",
            Description = "The FQDN of the zone"
        };
        var zoneID = new Option<uint>("--zoneID")
        {
            Name = "ID",
            Description = "The ID of the zone"
        };


        Add(zoneName);
        Add(zoneID);

        this.SetHandler(ExecuteCommand, zoneName, zoneID);
    }


    public async Task ExecuteCommand(string zoneName, uint zoneId)
    {
        Console.WriteLine($"Creating {zoneName}....");
        var trySetRecord = await _apiBroker.SetZoneInfoAsync(new Zone
        {
            // Should probably clean this up eventually, either by making the user enter an ID or grab the latest zone + 1 (problems though since CouchDB isn't made for that kind of auto incrementing value)
            ZoneId = zoneId == default ? (uint)Random.Shared.NextInt64(uint.MinValue, uint.MaxValue) : zoneId,
            ID = zoneName,
            Type = "NATIVE"
        });
        var setNewZoneResult = await trySetRecord.ProcessHttpResponseAsync<OperationResult>("new zone");
        if (setNewZoneResult == null)
        {
            Console.WriteLine("Errored out setting new zone");
            return;
        }

        Console.WriteLine($"Successfully created new zone {setNewZoneResult.Id}");
    }
}