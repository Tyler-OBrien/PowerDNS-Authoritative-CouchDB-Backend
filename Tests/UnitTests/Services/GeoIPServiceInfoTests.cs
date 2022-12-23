using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PowerDNS_Auth_CouchDB_Remote_Backend.Brokers;
using PowerDNS_Auth_CouchDB_Remote_Backend.Models;
using PowerDNS_Auth_CouchDB_Remote_Backend.Services;

namespace UnitTests.Services
{
    public class GeoIPServiceInfoTests
    {

        private readonly ILogger<IP2LocationGeoService> _logger;
        private readonly Mock<ILogger<IP2LocationGeoService>> _mockLogger;

        private readonly IP2LocationGeoService _zoneInfoService;

        public GeoIPServiceInfoTests()
        {
            _mockLogger = new Mock<ILogger<IP2LocationGeoService>>();
            _logger = _mockLogger.Object;
            _zoneInfoService = new IP2LocationGeoService(_logger);
        }

        [Test]
        [AutoData]
        public async Task TestGetRecordNoFilter(List<Record> records)
        {
            // Act
            var response = await _zoneInfoService.ProcessGeoIp(records, string.Empty);
            //Assert
            response.Should().NotBeNull("Response should not be null");
            response.Should().BeEquivalentTo(records);
        }
    }
}
