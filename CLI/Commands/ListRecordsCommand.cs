using System.CommandLine;
using CLI.Broker;
using CLI.Extensions;
using CLI.Models.DTOs;

namespace CLI.Commands;

public class ListRecordsCommand : Command
{
    private readonly IAPIBroker _apiBroker;

    public ListRecordsCommand(IAPIBroker apiBroker) : base("list-zone", "List records in a zone")
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
        Console.WriteLine($"Listing {zoneName} records...");
        var tryFetchZone = await _apiBroker.GetZoneInfoAsync(zoneName);


        var zoneInfo = await tryFetchZone.ProcessHttpResponseAsync<Zone>("zone info");
        if (zoneInfo == null)
        {
            Console.WriteLine("Could not get zone info from API");
            return;
        }


        var tryListRecords = await _apiBroker.ListRecordAsync(zoneInfo.ZoneId);


        var allRecords = await tryListRecords.ProcessHttpResponseAsync<List<Record>>("list records");
        if (allRecords == null)
        {
            Console.WriteLine("Could not get all records from API");
            return;
        }

        if (allRecords.Any() == false)
        {
            Console.WriteLine("Zone found, but no records found.");
            return;
        }

        // This is messy, we're trying to dynamically align the tabs based on the length of the tabs (all the other types are fairly constant)
        var maxLengthContent = allRecords.Select(i => i.Content.Length).Max();
        var maxLengthName = allRecords.Select(i => i.Name.Length).Max();


        Console.WriteLine(
            $"ID\t\t\t\t\tType\tName{addPadding(maxLengthName)}\tContent\t{addPadding(maxLengthContent)}TTL\tFlag");
        foreach (var record in allRecords)
            Console.WriteLine(
                $"{record.ID}\t{record.Type:6,1}\t{record.Name + addPadding(maxLengthName, record.Name.Length)}\t\t{'\"' + record.Content + '\"' + addPadding(maxLengthContent, record.Content.Length)}\t{record.TTL}\t{record.Flag}");
    }

    public static string addPadding(int padding, int currentPadding = 0)
    {
        var neededPadding = padding - currentPadding;
        if (neededPadding < 0)
            return string.Empty;
        return new string(' ', neededPadding);
    }
}