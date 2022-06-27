using System.CommandLine;
using CLI.Broker;
using CLI.Extensions;
using CLI.Models.DTOs;

namespace CLI.Commands;

public class CreateRecordCommand : Command
{
    private readonly IAPIBroker _apiBroker;

    public CreateRecordCommand(IAPIBroker apiBroker) : base("add-record",
        "Add a DNS record\nadd-record example.com @ AAAA 2a01:4f9:c010:30f4::1 --ttl=60 --flag")
    {
        _apiBroker = apiBroker;
        var zoneName = new Argument<string>("Zone")
        {
            Name = "Zone",
            Description = "FQDN of the record (example.com)"
        };

        var name = new Argument<string>("Target")
        {
            Name = "Target",
            Description = "The Target or name of the record (@ for Apex, or the subdomain like www)"
        };
        var type = new Argument<string>("Type")
        {
            Name = "Type",
            Description = "The Record Type (A, AAAA, CNAME)"
        };
        var content = new Argument<string>("Content")
        {
            Name = "Content",
            Description = "The content of the record"
        };
        var ttl = new Option<int>("--ttl")
        {
            Name = "TTL",
            Description = "The time to live of the record",
            IsRequired = false
        };
        var flag = new Option<string>("--flag")
        {
            Name = "Flag",
            Description =
                "Flag containing extra data for that record. Including if that value is based on GEOIP (GEOIP=NY) or LATENCY (LATENCY)",
            IsRequired = false
        };

        Add(zoneName);
        Add(name);
        Add(type);
        Add(content);
        Add(ttl);
        Add(flag);
        this.SetHandler(ExecuteCommand, zoneName, name, type, content, ttl, flag);
    }

    public async Task ExecuteCommand(string zoneName, string name, string type, string content, int ttl, string? flag)
    {
        if (ttl == 0) ttl = 3600; // 60 mins
        Console.WriteLine($"Creating.. {zoneName} {name} {type} {content} {ttl} {flag}");

        var tryFetchZone = await _apiBroker.GetZoneInfoAsync(zoneName);


        var zoneInfo = await tryFetchZone.ProcessHttpResponseAsync<Zone>("zone info");
        if (zoneInfo == null)
        {
            Console.WriteLine("Could not get zone info from API");
            return;
        }

        var fullyQualifiedRecordName = $"{name}.{zoneName}";
        if (name == "@") fullyQualifiedRecordName = zoneName;

        // Trim any trailing dots (i.e turn example.com. into example.com) and any whitespace
        fullyQualifiedRecordName = fullyQualifiedRecordName.TrimEnd('.').Trim();


        var trySetRecord = await _apiBroker.SetRecordAsync(new Record
        {
            Name = fullyQualifiedRecordName, Auth = true, Content = content, Disabled = false, Type = type, Flag = flag,
            TTL = ttl,
            zoneId = zoneInfo.ZoneId
        });
        var setRecordResult = await trySetRecord.ProcessHttpResponseAsync<OperationResult>("new record");
        if (setRecordResult == null)
        {
            Console.WriteLine("Errored out setting new record");
            return;
        }

        Console.WriteLine($"Successfully created new record - {setRecordResult.Id}");
    }
}