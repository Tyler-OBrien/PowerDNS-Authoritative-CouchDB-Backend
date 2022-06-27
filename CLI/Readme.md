Commands:

  add-record <Zone> <Target> <Type> <Content> -  Add a DNS record
  
  Example:                                     add-record example.com @ AAAA 2a01:4f9:c010:30f4::1 --ttl=60 --flag
                                               
  create-zone <Name>             -              Create a zone
  
  delete-record <recordId>        -             Delete a record from a zone
  
  delete-zone <Name>               -            Delete zone
  
  list-zone <Name>                  -           List records in a zone
  
  list-zones                   -                List zones
  

i.e

```bash
./CLI create-zone example.com
./CLI add-record example.com @ A 192.168.1.2
./CLI add-record example.com @ AAAA 2a01:4f9:c010:30f4::1
./CLI add-record example.com www A 192.168.1.2
./CLI add-record example.com NS A 192.168.1.2
./CLI add-record example.com @ SOA "ns.example.com. webadm.example.com. 3 10000 2400 604800 3600"
./CLI list-zone example.com
./CLI list-zones
./CLI delete-record <record ID got from list-zone>
./CLI delete-zone example.com (Doesn't require all records to be deleted)
```
