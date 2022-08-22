namespace PowerDNS_Auth_CouchDB_Remote_Backend.Models.Configuration;

public class ApplicationConfig
{
    public const string Section = "Application";
    public string CouchDB_Username { get; set; }
    public string CouchDB_Password { get; set; }
    public string CouchDB_URL { get; set; }
    public string SENTRY_DSN { get; set; }

    public int Prometheus_Metrics_Port { get; set; }

    public string CouchDB_Records_Database { get; set; }

    public string CouchDB_Zones_Database { get; set; }

    public string UnixSocketFile { get; set; }
}