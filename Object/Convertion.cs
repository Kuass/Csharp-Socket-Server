using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace socket_server.Object
{
    public class Convertion
    {
        public static void Temp_PacketPrintHeader(byte[] packet) {
            byte[] unit_type = ArraySortCopy(packet, Constants.INDEX.Type, Constants.LENGTH.Type);
            byte[] unit_placeid = ArraySortCopy(packet, Constants.INDEX.PlaceID, Constants.LENGTH.PlaceID);
            byte[] unit_deviceid = ArraySortCopy(packet, Constants.INDEX.DeviceID, Constants.LENGTH.DeviceID);
            byte[] unit_ins = ArraySortCopy(packet, Constants.INDEX.INS, Constants.LENGTH.INS);
            byte[] unit_ml = ArraySortCopy(packet, Constants.INDEX.ML, Constants.LENGTH.ML);

            string units_type = ReverseArrayBitConvertInt(unit_type).ToString();
            string units_placeid = Encoding.ASCII.GetString(unit_placeid);
            string units_deviceid = ReverseArrayBitConvertInt(unit_deviceid).ToString();
            string units_ins = Encoding.ASCII.GetString(unit_ins);
            string units_ml = ReverseArrayBitConvertInt(unit_ml).ToString();

            Server.print(2, "[HEADER]");
            Server.print(2, "Send DateTime - " + ByteArrayToHexString(packet, Constants.INDEX.SendDT, Constants.LENGTH.SendDT));
            Server.print(2, "SequenceNumber - " + ByteArrayToHexString(packet, Constants.INDEX.SEQ, Constants.LENGTH.SEQ));
            Server.print(2, "ChargerType - " + units_type + " | " + ByteArrayToHexString(unit_type));
            Server.print(2, "ChargePlaceID - " + units_placeid + " | " + ByteArrayToHexString(unit_placeid));
            Server.print(2, "ChargerID - " + units_deviceid + " | " + ByteArrayToHexString(unit_deviceid));
            Server.print(2, "INS - " + units_ins + " | " + ByteArrayToHexString(unit_ins));
            Server.print(2, "ML - " + units_ml + " | " + ByteArrayToHexString(unit_ml));
        }
        public static string ByteArrayToHexString(byte[] ba) => BitConverter.ToString(ba).Replace("-", "");
        public static string ByteArrayToHexString(byte[] ba, int startIndex, int length) {
            byte[] array = new byte[length];
            Array.Copy(ba, startIndex, array, 0, length);
            return BitConverter.ToString(array).Replace("-", "");
        }
        public static byte[] ReverseArray(byte[] ba) {
            byte[] array = ba;
            Array.Reverse(array);
            return array;
        }
        public static int ReverseArrayBitConvertInt(byte[] ba) {
            byte[] array = ba;
            Array.Reverse(array);
            int re = BitConverter.ToInt16(array);
            return re;
        }
        public static byte[] ArraySortCopy(byte[] ba, int startIndex, int length) {
            byte[] array = new byte[length];
            Array.Copy(ba, startIndex, array, 0, length);
            if (length == 1) {
                List<byte> arr = new List<byte>();
                arr.Add(0x00);
                arr.Add(array[0]);
                return arr.ToArray();
            }
            return array;
        }
        public static string WattageByteArrayToWattageString(byte[] data, bool t) // ex : 0x00002711
        {
            Array.Reverse(data);
            string temp = Convert.ToString(BitConverter.ToInt32(data)); // temp = 10001
            string min = temp.Substring(temp.Length - 2, 2); // 01
            string whole = temp.Substring(0, temp.Length - 2); // 100
            return whole + "." + min;
        }
        public static string WattageByteArrayToWattageString(byte[] data) // ex : 0x00002711
        {
            string temp = Convert.ToString(BitConverter.ToInt32(data)); // temp = 10001
            string min = temp.Substring(temp.Length - 2, 2); // 01
            string whole = temp.Substring(0, temp.Length - 2); // 100
            return whole + "." + min;
        }
        public static DateTime SendDateTimeBCDToDateTime(byte[] bcd) {
            byte[] year = new byte[2];
            byte[] month = new byte[1];
            byte[] day = new byte[1];
            byte[] hour = new byte[1];
            byte[] min = new byte[1];
            byte[] sec = new byte[1];
            Array.Copy(bcd, 0, year, 0, 2);
            Array.Copy(bcd, 2, month, 0, 1);
            Array.Copy(bcd, 3, day, 0, 1);
            Array.Copy(bcd, 4, hour, 0, 1);
            Array.Copy(bcd, 5, min, 0, 1);
            Array.Copy(bcd, 6, sec, 0, 1);
            DateTime dt = new DateTime(Convert.ToInt32(BCDToString(year)), Convert.ToInt32(BCDToString(month)), Convert.ToInt32(BCDToString(day)),
                Convert.ToInt32(BCDToString(hour)), Convert.ToInt32(BCDToString(min)), Convert.ToInt32(BCDToString(sec)));
            return dt;
        }
        public static string ByteArraytoStringWithDash(byte[] data) {
            StringBuilder temp = new StringBuilder(data.Length);
            for (int i = 0; i < data.Length; i++)
                temp.Append(data[i].ToString() + " ");
            return temp.ToString();
        }
        public static string ByteArraytoStringNoDash(byte[] data) {
            StringBuilder temp = new StringBuilder(data.Length);
            for (int i = 0; i < data.Length; i++)
                temp.Append(data[i].ToString());
            return temp.ToString();
        }
        public static int ByteToInt(byte[] data) => (data[0] << 8 | data[1]) & 0x0000ffff;
        public static byte[] IntToByteArray(int data, int length) {
            byte[] array = new byte[length];
            switch (length) {
                case 1:
                    array[0] = (byte)(0x000000ff & data);
                    break;
                case 2:
                    array[0] = (byte)(0x000000ff & (data >> 8));
                    array[1] = (byte)(0x000000ff & (data));
                    break;
                case 3:
                    array[0] = (byte)(0x000000ff & (data >> 16));
                    array[1] = (byte)(0x000000ff & (data >> 8));
                    array[2] = (byte)(0x000000ff & (data));
                    break;
                case 4:
                    array[0] = (byte)(0x000000ff & (data >> 24));
                    array[1] = (byte)(0x000000ff & (data >> 16));
                    array[2] = (byte)(0x000000ff & (data >> 8));
                    array[3] = (byte)(0x000000ff & (data));
                    break;
                case 5:
                    array[0] = (byte)(0x000000ff & (data >> 32));
                    array[1] = (byte)(0x000000ff & (data >> 24));
                    array[2] = (byte)(0x000000ff & (data >> 16));
                    array[3] = (byte)(0x000000ff & (data >> 8));
                    array[4] = (byte)(0x000000ff & (data));
                    break;
                case 6:
                    array[0] = (byte)(0x000000ff & (data >> 40));
                    array[1] = (byte)(0x000000ff & (data >> 32));
                    array[2] = (byte)(0x000000ff & (data >> 24));
                    array[3] = (byte)(0x000000ff & (data >> 16));
                    array[4] = (byte)(0x000000ff & (data >> 8));
                    array[5] = (byte)(0x000000ff & (data));
                    break;
                case 7:
                    array[0] = (byte)(0x000000ff & (data >> 48));
                    array[1] = (byte)(0x000000ff & (data >> 40));
                    array[2] = (byte)(0x000000ff & (data >> 32));
                    array[3] = (byte)(0x000000ff & (data >> 24));
                    array[4] = (byte)(0x000000ff & (data >> 16));
                    array[5] = (byte)(0x000000ff & (data >> 8));
                    array[6] = (byte)(0x000000ff & (data));
                    break;
                case 8:
                    array[0] = (byte)(0x000000ff & (data >> 56));
                    array[1] = (byte)(0x000000ff & (data >> 48));
                    array[2] = (byte)(0x000000ff & (data >> 40));
                    array[3] = (byte)(0x000000ff & (data >> 32));
                    array[4] = (byte)(0x000000ff & (data >> 24));
                    array[5] = (byte)(0x000000ff & (data >> 16));
                    array[6] = (byte)(0x000000ff & (data >> 8));
                    array[7] = (byte)(0x000000ff & (data));
                    break;
                default:
                    byte[] a = { (byte)'0' };
                    return a;
            }
            return array;
        }
        public static List<byte> StringToHex(string data) {
            List<byte> hexList = new List<byte>();
            for (int i = 0; i < data.Length - 1; i += 2)
                hexList.Add(StringToHex(data[i], data[i + 1]));
            return hexList;
        }
        public static byte StringToHex(char first, char second) {
            byte hex = Convert.ToByte(first.ToString(), 16);
            hex <<= 4;
            hex += Convert.ToByte(second.ToString(), 16);
            return hex;
        }
        public static byte[] Combine(byte[][] combineData) {
            int length = 0;
            for (int i = 0; i < combineData.Length; i++) {
                if (combineData[i] == null) continue;
                length += combineData[i].Length;
            }
            byte[] combine = new byte[length];

            int combineDataLength = combineData.Length;
            int index = 0;
            for (int i = 0; i < combineDataLength; i++) {
                if (combineData[i] == null) continue;

                for (int j = 0; j < combineData[i].Length; j++) {
                    combine[index] = combineData[i][j];
                    index += 1;
                }
            }
            return combine;
        }
        public static string BCDToString(byte[] bytes) {
            StringBuilder temp = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++) {
                temp.Append((byte)((bytes[i] & 0xf0) >> 4));
                temp.Append((byte)(bytes[i] & 0x0f));
            }

            return temp.ToString().Substring(0, 1).Equals("0") ? temp.ToString().Substring(1) : temp.ToString();
        }
        public static string StringToBCD(bool isLittleEndian, params byte[] bytes) {
            StringBuilder bcd = new StringBuilder(bytes.Length * 2);

            if (isLittleEndian) {
                for (int i = bytes.Length - 1; i >= 0; i--) {
                    byte bcdByte = bytes[i];
                    int idHigh = bcdByte >> 4;
                    int idLow = bcdByte & 0x0F;
                    if (idHigh > 9 || idLow > 9) return null;
                    bcd.Append(string.Format("{0}{1}", idHigh, idLow));
                    //throw new ArgumentException(String.Format("One of the argument bytes was not in binary-coded decimal format: byte[{0}] = 0x{1:X2}.", i, bcdByte));
                }
            } else {
                for (int i = 0; i < bytes.Length; i++) {
                    byte bcdByte = bytes[i];
                    int idHigh = bcdByte >> 4;
                    int idLow = bcdByte & 0x0F;
                    if (idHigh > 9 || idLow > 9) return null;
                    //throw new ArgumentException(String.Format("One of the argument bytes was not in binary-coded decimal format: byte[{0}] = 0x{1:X2}.", i, bcdByte));
                    bcd.Append(string.Format("{0}{1}", idHigh, idLow));
                }
            }
            return bcd.ToString();
        }





        public class Crc16
        {
            public static ushort CalcCRC(byte[] strPacket) {
                ushort[] CRC16_TABLE = { 0x0000, 0xCC01, 0xD801, 0x1400, 0xF001, 0x3C00, 0x2800, 0xE401, 0xA001, 0x6C00, 0x7800, 0xB401, 0x5000, 0x9C01, 0x8801, 0x4400 };
                ushort usCRC = 0xFFFF;
                ushort usTemp = 0;

                foreach (char cCurrent in strPacket) {
                    byte bytCurrent = Convert.ToByte(cCurrent);// lower 4 bits
                    usTemp = CRC16_TABLE[usCRC & 0x000F];
                    usCRC = (ushort)((usCRC >> 4) & 0x0FFF);
                    usCRC = (ushort)(usCRC ^ usTemp ^ CRC16_TABLE[bytCurrent & 0x000F]); // Upper 4 Bits
                    usTemp = CRC16_TABLE[usCRC & 0x000F];
                    usCRC = (ushort)((usCRC >> 4) & 0x0FFF);
                    usCRC = (ushort)(usCRC ^ usTemp ^ CRC16_TABLE[(bytCurrent >> 4) & 0x000F]);
                }

                return usCRC;
            }

            public static byte[] getCrc16(byte[] packet) {
                int crc = (int)0xffff;

                for (int i = 1; i < packet.Length - 3; i++)
                    crc = (short)(((crc >> 8) & 0x00ff) + ((crc ^ packet[i]) & 0x00ff));

                byte[] crc_Bytes = new byte[2];

                crc_Bytes[0] = (byte)((crc >> 8) & 0x000000ff);
                crc_Bytes[1] = (byte)(crc & 0x000000ff);

                return crc_Bytes;
            }
        }
    }
}
