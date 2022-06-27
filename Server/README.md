# Server (PowerDNS CouchDB Backend)

This exposes endpoints for PowerDNS (/dns) and Rest Endpoints for clients like the CLI tool (/v1/dnsapi)

Example configuration:
```json
{
  "AllowedHosts": "*",
  "Application": {
    "CouchDB_Username": "admin",
    "CouchDB_Password": "password",
    "CouchDB_URL": "http://localhost:5984",
    // If blank, won't be used
    "SENTRY_DSN": "",
    "CouchDB_Records_Database": "dns_records",
    "CouchDB_Zones_Database": "dns_zones",
    // If 0, won't be used
    "Prometheus_Metrics_Port": 9789
  },
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://127.0.0.1:5112",
        "Protocols": "Http1AndHttp2"
      },
      // Not needed if Prometheus metrics port is 0 / disabled
      "HttpPrometheus": {
        "Url": "http://127.0.0.1:9789",
        "Protocols": "Http1AndHttp2"
      }
    }
  }
}
```
