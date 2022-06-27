A PowerDNS Authoritative Backend using CouchDB (intended for use with multiple regions, using CouchDB Replication)

This uses the PowerDNS Remote Backend and contains only the minimum amount of endpoints for functionality (lookup, get domain metadata, and get all domains).

The default pdnsutil requires transaction support for creating zones, so there's a small CLI tool included in this project that matches the commands of pdnsutil (add-record, list-zone, list-zones, create-zone). 

There is no security or required API Keys, as this is intended to be not exposed to the Public Internet and only used by PowerDNS's Remote Backend and the CLI tool.

Requires PowerDNS Remote Backend

Example pdns.conf:
```
launch=remote
remote-connection-string=http:url=http://127.0.0.1:5112/dns,timeout=20000
```