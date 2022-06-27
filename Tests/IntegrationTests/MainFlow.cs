using System.Net.Http.Json;
using System.Text.Json;
using AutoFixture.NUnit3;
using CLI.Extensions;
using FluentAssertions;
using IntegrationTests.Fixtures;
using NUnit.Framework;
using PowerDNS_Auth_CouchDB_Remote_Backend;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.API_Responses;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Responses.PowerDNS_Responses.Lookup;

namespace IntegrationTests;

public class MainFlow
{
    public const string ZonesDB = "/v1/dnsapi/Zone";
    public const string RecordsDB = "/v1/dnsapi/Record";
    private readonly HttpClient _client;

    public MainFlow()
    {
        var factory = new TestingWebAppFactory<Program>();
        _client = factory.CreateClient();
    }

    [Test]
    [AutoData]
    public async Task MainFlowTest(Zone zone, Record record)
    {
        // Data
        record.zoneId = zone.ZoneId;

        // First try creating the zone
        var tryInsertZone = await _client.PostAsJsonAsync(ZonesDB, zone);


        var tryInsertZoneStringResponse = await tryInsertZone.Content.ReadAsStringAsync();

        tryInsertZone.IsSuccessStatusCode.Should()
            .BeTrue(
                $"We should be able to create new zone, got response {tryInsertZoneStringResponse}");

        // Then try fetching the zone

        var tryGetZone = await _client.GetAsync($"{ZonesDB}/{Uri.EscapeDataString(zone.ID)}");

        var tryGetZoneStringResponse = await tryGetZone.Content.ReadAsStringAsync();

        tryGetZone.IsSuccessStatusCode.Should()
            .BeTrue(
                $"We should be able to get our zone, we got back {tryGetZoneStringResponse}");

        var getZone =
            JsonSerializer.Deserialize<DataResponse<Zone>>(tryGetZoneStringResponse);

        getZone.Data.Should().NotBeNull("Zone should not be null");
        getZone.Data.Should().BeEquivalentTo(zone, "Zone should equal zone we inserted");

        // Try add record
        var tryInsertRecord = await _client.PostAsJsonAsync(RecordsDB, record);

        tryInsertRecord.IsSuccessStatusCode.Should().BeTrue(
            $"We should be able to create a new record, got response {await tryInsertRecord.Content.ReadAsStringAsync()}");

        // Try Get Record from DNS API

        var tryGetRecordByID =
            await _client.GetFromJsonAsync<DataResponse<Record>>($"{RecordsDB}/ID/{Uri.EscapeDataString(record.ID)}");

        tryGetRecordByID.Data.Should()
            .BeEquivalentTo(record, "Record we get should be the same as the one we inserted");

        // Try Get Record from PDNS Lookup API

        var tryGetRecordFromPDNSAPI =
            await _client.GetFromJsonAsync<LookupResponse>(
                $"dns/lookup/{Uri.EscapeDataString(record.Name)}/{Uri.EscapeDataString(record.Type)}");

        tryGetRecordFromPDNSAPI.Should().NotBeNull("Response from PDNS API should not be null");

        tryGetRecordFromPDNSAPI.Result.Should().Contain(
            pdnsRecord => pdnsRecord.QName.Equals(record.Name, StringComparison.OrdinalIgnoreCase) &&
                          pdnsRecord.QType.Equals(record.Type, StringComparison.OrdinalIgnoreCase),
            "PDNS Response should return our record");

        // Now Delete

        var tryDeleteRecord = await _client.DeleteAsJsonAsync(RecordsDB, record);

        tryDeleteRecord.IsSuccessStatusCode.Should().BeTrue(
            $"We should be able to delete our Record, we got back {await tryDeleteRecord.Content.ReadAsStringAsync()}");

        var tryDeleteZone = await _client.DeleteAsJsonAsync(ZonesDB, zone);

        tryDeleteZone.IsSuccessStatusCode.Should()
            .BeTrue(
                $"We should be able to delete our Zone, we got back {await tryDeleteZone.Content.ReadAsStringAsync()}");
    }
}