using socket_server.Object;
using System;
using System.Text;

namespace socket_server
{
    public class DataReceive
    {
        public static bool Get(StateObject state, int length) {
            byte[] packet = new byte[length];
            Array.Copy(state.buffer, 0, packet, 0, length);
            if (length < Constants.VALUE.LENGTH_TOTAL ||
                state.buffer[0] != Constants.VALUE.STX ||
                state.buffer[length - 1] != Constants.VALUE.ETX)
                return true;

            byte[] cut = new byte[length - 4];
            Array.Copy(packet, 1, cut, 0, length - 4);
            byte[] crc_result = BitConverter.GetBytes(Convertion.Crc16.CalcCRC(cut));

            if (Server.DebugLevel > 2) Server.print(2, "Receive Data - " + Convertion.ByteArrayToHexString(packet));

            if (packet[length - 3] != crc_result[1] &&
                packet[length - 2] != crc_result[0]) {
                if (Server.DebugLevel > 2) Server.print(2, "CRC eRROR");
                return true;
            }

            byte[] unit_type = new byte[0], unit_placeid = new byte[0], unit_deviceid = new byte[0];
            byte[] unit_ins = new byte[0], unit_ml = new byte[0];
            string units_type = "", units_placeid = "", units_deviceid = "", units_ins = "", units_ml = "";

            try {
                unit_type = Convertion.ArraySortCopy(packet, Constants.INDEX.Type, Constants.LENGTH.Type);
                unit_placeid = Convertion.ArraySortCopy(packet, Constants.INDEX.PlaceID, Constants.LENGTH.PlaceID);
                unit_deviceid = Convertion.ArraySortCopy(packet, Constants.INDEX.DeviceID, Constants.LENGTH.DeviceID);
                unit_ins = Convertion.ArraySortCopy(packet, Constants.INDEX.INS, Constants.LENGTH.INS);
                unit_ml = Convertion.ArraySortCopy(packet, Constants.INDEX.ML, Constants.LENGTH.ML);

                units_type = Convertion.ReverseArrayBitConvertInt(unit_type).ToString();
                units_placeid = Encoding.ASCII.GetString(unit_placeid);
                units_deviceid = Convertion.ReverseArrayBitConvertInt(unit_deviceid).ToString();
                units_ins = Encoding.ASCII.GetString(unit_ins);
                units_ml = Convertion.ReverseArrayBitConvertInt(unit_ml).ToString();
            } catch (Exception) {
                if (Server.DebugLevel > 2) Server.print(2, Detail.get(state, Detail.TYPE.Addr) + " - Unrecognized Packets");
                return true;
            }

            if (Server.DebugLevel > 2) {
                Convertion.Temp_PacketPrintHeader(packet);
                Server.print(2, "CRC oK");
            }

            if (units_ins != "a1" && !state.thru) {
                if (Server.DebugLevel > 2) Server.print(2, Detail.get(state, Detail.TYPE.Addr) + " - proceed without approval.");
                return false;
            }

            string[] data = new string[] { units_placeid, units_deviceid, Detail.get(state, Detail.TYPE.Addr), units_ins };
            Database.Charger.Record.LeaveDataRaw(false, data, packet, false);
            switch (units_ins) {
                case "a1":
                    state.thru = true;
                    Detail.set(state, Detail.TYPE.Type, units_type);
                    Detail.set(state, Detail.TYPE.PlaceID, units_placeid);
                    Detail.set(state, Detail.TYPE.DeviceID, units_deviceid);
                    Server.print(0, Detail.get(state, Detail.TYPE.Addr) + " is vertify success - " + units_placeid + ", " + units_deviceid);
                    data = new string[] { "1a" };
                    DataSend.Get(state, data);
                    Database.Charger.Session.Join(state);
                    return true;
                default:
                    return true;
            }
        }
    }
}
