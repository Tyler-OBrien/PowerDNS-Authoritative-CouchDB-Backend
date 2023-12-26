using IP2Location;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Maxmind;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Services;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Services;

public class IP2LocationGeoService : IGeoIPService
{
    private const string DB3_IPV6 = "IP2LOCATION-LITE-DB3.IPV6.BIN";


    private const string DB3_IPV4 = "IP2LOCATION-LITE-DB3.BIN";
    private readonly Component? _dbReaderIPv4;
    private readonly Component? _dbReaderIPv6;

    private readonly ILogger _logger;
    private readonly IPTools _tools;

    public IP2LocationGeoService(ILogger<IP2LocationGeoService> logger)
    {
        _logger = logger;
        _tools = new IPTools();
        if (File.Exists(DB3_IPV4) == false)
        {
            _logger.LogWarning("Can't find IPv4 File for IP2Location Service... will not grab IPv4 Info");
            _dbReaderIPv4 = null;
        }
        else
        {
            _dbReaderIPv4 = new Component();
            _dbReaderIPv4.Open(DB3_IPV4, true);
        }

        if (File.Exists(DB3_IPV6) == false)
        {
            _logger.LogWarning("Can't find IPv6 File for IP2Location Service... will not grab IPv6 Info");
            _dbReaderIPv6 = null;
        }
        else
        {
            _dbReaderIPv6 = new Component();
            _dbReaderIPv6.Open(DB3_IPV6, true);
        }
    }


    public ValueTask<IPInformation> GetIpInformation(string address)
    {
        var newIpInformation = new IPInformation();
        IPResult ipResult = null;
        if (_tools.IsIPv4(address))
        {
            if (_dbReaderIPv4 != null) ipResult = _dbReaderIPv4.IPQuery(address);
        }
        else if (_tools.IsIPv6(address))
        {
            if (_dbReaderIPv6 != null) ipResult = _dbReaderIPv6.IPQuery(address);
        }


        if (ipResult != null)
        {
            newIpInformation.Continent = ipResult.Region;
            newIpInformation.Latitude = ipResult.Latitude;
            newIpInformation.Longitude = ipResult.Longitude;
            newIpInformation.TimeZone = ipResult.TimeZone;
            newIpInformation.City = ipResult.City;
            newIpInformation.Country = ipResult.CountryLong;

            newIpInformation.CountryCodeISO = ipResult.CountryShort;

            newIpInformation.ContinentCode = ipResult.AreaCode;
        }

        return ValueTask.FromResult(newIpInformation);
    }

    public async ValueTask<List<Record>> ProcessGeoIp(List<Record> records, string remoteIp)
    {
        // For Compat with Lookup mutiple types, find distinct types and targets and limit off that
        var output = new List<Record>();
        // Should only really be one name, but just in case..
        var recordsByName = records.GroupBy(record => record.Name).Select(record => record.ToList()).ToList();
        foreach (var recordGroupedByName in recordsByName)
            // Then by type
        foreach (var recordGroupedByType in recordGroupedByName.GroupBy(record => record.Type)
                     .Select(record => record.ToList()).ToList())
            output.AddRange(await ProcessGeoIp_Internal_PerType(recordGroupedByType, remoteIp));
        return output;
    }

    private async ValueTask<List<Record>> ProcessGeoIp_Internal_PerType(List<Record> records, string remoteIp)
    {
        var findRecordsWithGeoTags =
            records.Where(record => record?.Flag?.StartsWith("Geo=", StringComparison.OrdinalIgnoreCase) ?? false)
                .ToList();
        if (string.IsNullOrWhiteSpace(remoteIp) == false && (findRecordsWithGeoTags?.Any() ?? false))
        {
            var matchedRecords = new List<Record>();
            // PowerDNS sends in Edns as CIDR notation
            if (remoteIp.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) != -1)
                remoteIp = remoteIp.Substring(0, remoteIp.LastIndexOf("/", StringComparison.OrdinalIgnoreCase));


            var info = await GetIpInformation(remoteIp);
            foreach (var record in findRecordsWithGeoTags)
            {
                // Kind of messy, but try to match the Flagw ith the Continent or Country
                var Geo = new HashSet<string>(
                    record!.Flag!.Substring(record.Flag.IndexOf("Geo=", StringComparison.OrdinalIgnoreCase))
                        .Split(","), StringComparer.OrdinalIgnoreCase);
                if (Geo.Contains(info.CountryCodeISO ?? "Unknown")) matchedRecords.Add(record);
            }

            if (matchedRecords.Any()) return matchedRecords;
        }

        // Return all records without Geo Tags 
        if (findRecordsWithGeoTags?.Any() ?? false) return records.Except(findRecordsWithGeoTags).ToList();

        return records;
    }
}