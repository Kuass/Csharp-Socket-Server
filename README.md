# Base Socket Server
Created with `C#` `Net Core 3.1`, i believe it will be the foundation of a powerful server application through your touch.

Reads and sends packet according to ASCII standards and disconnects socket connections that have not been authenticated after 30 seconds.
a1 Socket connections that successfully received INS are not disconnected after 30 seconds, and the state structure manages data for each session.

| Name         | STX | Send Datetime | Sequence Number | Type | Place ID | Device ID | INS  | Message Length | Data | CRC-16 Checksum | ETX      |
|--------------|-----|---------------|-----------------|------|----------|-----------|------|----------------|------|-----------------|----------|
| Short Name   | STX | SendDT        | SEQ             | Type | PlaceID  | DeviceID  | INS  | ML             | VD   | CRC             | ETX      |
| Length(byte) | 1   | 7             | 2               | 1    | 8        | 2         | 2    | 2              | N    | 2               | 1        |
| Index        | 1   | 8             | 10              | 11   | 19       | 21        | 23   | 25             | N    | 25+N+1          | 25+N+1+1 |
| Format       | ASC | BCD           | byte            | byte | byte     | byte      | byte | byte           |      | byte            | ASC      |

Header : STX, SendDT, SEQ, Type, PlaceID, DeviceID, INS, ML<br>
Body : VD<br>
Tail : CRC, ETX<br>

**The CRC-16 checksum algorithm uses ARC-CCITT.**<br>
❗ In order to commercialize this project, we will need to add the part that encrypts the packet.

## Building
- .Net Core 3.1

<br>

Build for ubuntu-18.04 : 
  `dotnet build --no-restore --configuration Release --runtime ubuntu.18.04-x64 -p:ImportByWildcardBeforeSolution=false`

Build for Windows : 
  `dotnet build --no-restore --configuration Release`

### Dependencies Packages
- MySql.Data v8.0.22 (Nuget)
