Certbot could still be used with this setup with a custom auth and clean up hook, and you'd just have to add any other hook you want (Nginx reload, etc)

For example, a simple bash script using JQ for JSON Parsing and curl for requests, depending on the CouchDB PowerDNS Backend being on localhost:5112

I had some issues with timeouts and cache, depending on your setup. 

certbot --manual --preferred-challenges=dns --manual-auth-hook /path/to/auth.sh --manual-cleanup-hook /path/to/cleanup.sh -d 'Your_Domain' -i nginx

auth.sh
```bash
#!/bin/bash

# Turn subdomain.example.com into example.com, or default to example.com
DOMAIN=$(expr match "$CERTBOT_DOMAIN" '.*\.\(.*\..*\)') || DOMAIN=$CERTBOT_DOMAIN
# Get Zone ID 
ZONE_ID=$(curl -s -X GET "http://localhost:5112/v1/dnsapi/Zone/$DOMAIN" -H     "Content-Type: application/json" | jq -r '.data.zoneId')
# Get fqdn of acme challenge
CREATE_DOMAIN="_acme-challenge.$CERTBOT_DOMAIN"

# Create the record and get the record ID
RECORD_ID=$(curl -s -X POST "http://localhost:5112/v1/dnsapi/Record" \
     -H     "Content-Type: application/json" \
     --data '{"type":"TXT","name":"'"$CREATE_DOMAIN"'","content":"'"$CERTBOT_VALIDATION"'","ttl":120, "zone_id": '$ZONE_ID', "auth": true, "disabled": false  }' \
             | jq -r '.data.id')


# Save info for cleanup
if [ ! -d /tmp/CERTBOT_$CERTBOT_VALIDATION ];then
        mkdir -m 0700 /tmp/CERTBOT_$CERTBOT_VALIDATION
fi
echo $ZONE_ID > /tmp/CERTBOT_$CERTBOT_VALIDATION/ZONE_ID
echo $RECORD_ID > /tmp/CERTBOT_$CERTBOT_VALIDATION/RECORD_ID
```

A cleanup script, deleting the record by grabbing the info from the tmp file

cleanup.sh
```bash
#!/bin/bash

if [ -f /tmp/CERTBOT_$CERTBOT_VALIDATION/ZONE_ID ]; then
        ZONE_ID=$(cat /tmp/CERTBOT_$CERTBOT_VALIDATION/ZONE_ID)
        rm -f /tmp/CERTBOT_$CERTBOT_VALIDATION/ZONE_ID
fi

if [ -f /tmp/CERTBOT_$CERTBOT_VALIDATION/RECORD_ID ]; then
        RECORD_ID=$(cat /tmp/CERTBOT_$CERTBOT_VALIDATION/RECORD_ID)
        rm -f /tmp/CERTBOT_$CERTBOT_VALIDATION/RECORD_ID
fi

# Remove the challenge TXT record from the zone
if [ -n "${ZONE_ID}" ]; then
    if [ -n "${RECORD_ID}" ]; then
	RECORD=$(curl -s -X GET "http://localhost:5112/v1/dnsapi/Record/ID/$RECORD_ID" -H     "Content-Type: application/json" | jq '.data')
        curl -s -X DELETE "http://localhost:5112/v1/dnsapi/Record" -H "Content-Type: application/json" --data "$RECORD"
    fi
fi

```