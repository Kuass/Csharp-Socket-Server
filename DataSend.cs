using socket_server.Object;
using System;
using System.Text;

namespace socket_server
{
    public class DataSend
    {
        public static void Get(StateObject state, string[] data)
        // [0] : ins
        {
            byte[] stx = new byte[1] { Constants.VALUE.STX };
            byte[] send_time = new byte[7];
            byte[] seq = new byte[2];
            byte[] type = new byte[1] { 0x06 };
            byte[] placeid = new byte[8] { 0x4D, 0x4F, 0x44, 0x45, 0x52, 0x4E, 0x54, 0x31 };
            byte[] deviceid = new byte[2] { 0x00, 0x01 };
            byte[] ins = new byte[2];
            byte[] ml = new byte[2];
            byte[] VD = null;
            byte[] crc = new byte[2];
            byte[] etx = new byte[1] { Constants.VALUE.ETX };


            type = Convertion.IntToByteArray(Convert.ToInt32(Detail.get(state, Detail.TYPE.Type)), 1);
            placeid = Encoding.ASCII.GetBytes(Detail.get(state, Detail.TYPE.PlaceID));
            deviceid = Convertion.IntToByteArray(Convert.ToInt32(Detail.get(state, Detail.TYPE.DeviceID)), 2);

            int a = 0;
            var n = DateTime.Now.ToString("yyyyMMddHHmmss");
            foreach (var bcd in Convertion.StringToHex(n)) {
                send_time[a++] = bcd;
            }

            ins = Encoding.ASCII.GetBytes(data[0]);
            switch (data[0]) {
                case "1a":
                    VD = new byte[2] { Constants.VALUE.ACK, 0x00 };
                    break;
                case "1b":
                    VD = new byte[2] { Constants.VALUE.ACK, 0x00 };
                    break;
                case "1c":
                    VD = new byte[2] { Constants.VALUE.ACK, 0x00 };
                    break;
            }

            if (VD != null) {
                ml[0] = (byte)(0x000000ff & (VD.Length >> 8));
                ml[1] = (byte)(0x000000ff & (VD.Length));
            }

            byte[][] combineData_Temp = new byte[][] { stx, send_time, seq, type, placeid, deviceid, ins, ml, VD, crc, etx };
            byte[] packet = Convertion.Combine(combineData_Temp);

            byte[] array = new byte[packet.Length - 4];
            Array.Copy(packet, 1, array, 0, packet.Length - 4);
            crc = BitConverter.GetBytes(Convertion.Crc16.CalcCRC(array));
            packet[packet.Length - 3] = crc[1];
            packet[packet.Length - 2] = crc[0];

            Server.Send(state.workSocket, packet);

            data = new string[] { Detail.get(state, Detail.TYPE.PlaceID),
                Detail.get(state, Detail.TYPE.DeviceID),
                Detail.get(state, Detail.TYPE.Addr),
                data[0] };
            Database.Charger.Record.LeaveDataRaw(true, data, packet, false);
        }
    }
}
