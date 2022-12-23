using PowerDNS_Auth_CouchDB_Remote_Backend.Models.Maxmind;

namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.Services
{
    public interface IGeoIPService
    {
        public ValueTask<IPInformation> GetIpInformation(string address);

        public ValueTask<List<Record>> ProcessGeoIp(List<Record> records, string IP);
    }
}
