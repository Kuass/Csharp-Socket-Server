namespace socket_server
{
    public class Constants
    {
        public class VALUE
        {
            public const byte STX = 0x02;
            public const byte ETX = 0x03;
            public const byte ACK = 0x06;
            public const byte NAK = 0x15;
            public const int LENGTH_TOTAL = 29;
        }

        public class LENGTH
        {
            public const int STX = 1;
            public const int SendDT = 7;
            public const int SEQ = 2;
            public const int Type = 1;
            public const int PlaceID = 8;
            public const int DeviceID = 2;
            public const int INS = 2;
            public const int ML = 2;
            public const int CRC = 2;
            public const int ETX = 1;
        }

        public class INDEX
        {
            public const int STX = 0;
            public const int SendDT = STX + 1;
            public const int SEQ = SendDT + 7;
            public const int Type = SEQ + 2;
            public const int PlaceID = Type + 1;
            public const int DeviceID = PlaceID + 8;
            public const int INS = DeviceID + 1;
            public const int ML = INS + 2;
            public const int VD = ML + 2;
        }
    }
}
