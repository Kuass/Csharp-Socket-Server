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

        public class Encrypt
        {
            public class ARIA
            {
                /// <summary>
                /// kr.re.nsri.aria.ARIAEngine JAVA클래스를 C#에 맞게 변환한 것
                /// 일부 변수 타입 차이, 연산자 차이등을 모두 맞추었음. 자바에서 암호화한 것을
                /// 복호화하거나 반대로 테스트한 결과 문제는 없는 것 같다.
                /// 
                /// 참고로 C#은 메소드 이름을 대문자로 시작하는 것이 일반적이므로 그 스타일에 맞게 변경함
                /// </summary>
                public class ARIAEngine
                {
                    private static readonly char[] HEX_DIGITS = new char[16] {
                '0','1','2','3','4','5','6','7','8','9','a','b','c','d','e','f'
            };

                    private static readonly uint[,] KRK = new uint[,] {
                {0x517cc1b7, 0x27220a94, 0xfe13abe8, 0xfa9a6ee0},
                {0x6db14acc, 0x9e21c820, 0xff28b1d5, 0xef5de2b0},
                {0xdb92371d, 0x2126e970, 0x03249775, 0x04e8c90e}
            };

                    private static readonly byte[] S1 = new byte[256];
                    private static readonly byte[] S2 = new byte[256];
                    private static readonly byte[] X1 = new byte[256];
                    private static readonly byte[] X2 = new byte[256];

                    private static readonly uint[] TS1 = new uint[256];
                    private static readonly uint[] TS2 = new uint[256];
                    private static readonly uint[] TX1 = new uint[256];
                    private static readonly uint[] TX2 = new uint[256];

                    // Static initializer.  For setting up the tables
                    static ARIAEngine() {
                        uint[] exp = new uint[256];
                        uint[] log = new uint[256];
                        exp[0] = 1;
                        for (int i = 1; i < 256; i++) {
                            uint j = (exp[i - 1] << 1) ^ exp[i - 1];
                            if ((j & 0x100) != 0) j ^= 0x11b;
                            exp[i] = j;
                        }
                        for (int i = 1; i < 255; i++)
                            log[exp[i]] = (uint)i;

                        uint[,] A = new uint[,] {
                    {1, 0, 0, 0, 1, 1, 1, 1},
                    {1, 1, 0, 0, 0, 1, 1, 1},
                    {1, 1, 1, 0, 0, 0, 1, 1},
                    {1, 1, 1, 1, 0, 0, 0, 1},
                    {1, 1, 1, 1, 1, 0, 0, 0},
                    {0, 1, 1, 1, 1, 1, 0, 0},
                    {0, 0, 1, 1, 1, 1, 1, 0},
                    {0, 0, 0, 1, 1, 1, 1, 1}
                };

                        uint[,] B = new uint[,] {
                    {0, 1, 0, 1, 1, 1, 1, 0},
                    {0, 0, 1, 1, 1, 1, 0, 1},
                    {1, 1, 0, 1, 0, 1, 1, 1},
                    {1, 0, 0, 1, 1, 1, 0, 1},
                    {0, 0, 1, 0, 1, 1, 0, 0},
                    {1, 0, 0, 0, 0, 0, 0, 1},
                    {0, 1, 0, 1, 1, 1, 0, 1},
                    {1, 1, 0, 1, 0, 0, 1, 1}
                };

                        for (int i = 0; i < 256; i++) {
                            uint t = 0, p;
                            if (i == 0)
                                p = 0;
                            else
                                p = exp[255 - log[i]];
                            for (int j = 0; j < 8; j++) {
                                uint s = 0;
                                for (int k = 0; k < 8; k++) {
                                    if (((p >> (7 - k)) & 0x01) != 0)
                                        s ^= A[k, j];
                                }
                                t = (t << 1) ^ s;
                            }
                            t ^= 0x63;
                            S1[i] = (byte)t;
                            X1[t] = (byte)i;
                        }
                        for (int i = 0; i < 256; i++) {
                            uint t = 0, p;
                            if (i == 0)
                                p = 0;
                            else
                                p = exp[(247 * log[i]) % 255];
                            for (int j = 0; j < 8; j++) {
                                uint s = 0;
                                for (int k = 0; k < 8; k++) {
                                    if (((p >> k) & 0x01) != 0)
                                        s ^= B[7 - j, k];
                                }
                                t = (t << 1) ^ s;
                            }
                            t ^= 0xe2;
                            S2[i] = (byte)t;
                            X2[t] = (byte)i;
                        }

                        for (int i = 0; i < 256; i++) {
                            TS1[i] = (uint)0x00010101 * (S1[i] & (uint)0xff);
                            TS2[i] = (uint)0x01000101 * (S2[i] & (uint)0xff);
                            TX1[i] = (uint)0x01010001 * (X1[i] & (uint)0xff);
                            TX2[i] = (uint)0x01010100 * (X2[i] & (uint)0xff);
                        }
                    }

                    private int keySize = 0;
                    private int numberOfRounds = 0;
                    private byte[] masterKey = null;
                    private uint[] encRoundKeys = null, decRoundKeys = null;

                    public ARIAEngine(int keySize) {
                        SetKeySize(keySize);
                    }

                    /**
                    * Resets the class so that it can be reused for another master key.
                    */
                    void Reset() {
                        this.keySize = 0;
                        this.numberOfRounds = 0;
                        this.masterKey = null;
                        this.encRoundKeys = null;
                        this.decRoundKeys = null;
                    }

                    int GetKeySize() {
                        return this.keySize;
                    }

                    void SetKeySize(int keySize) {
                        this.Reset();
                        if (keySize != 128 && keySize != 192 && keySize != 256)
                            throw new Exception("keySize=" + keySize);
                        this.keySize = keySize;
                        switch (keySize) {
                            case 128:
                                this.numberOfRounds = 12;
                                break;
                            case 192:
                                this.numberOfRounds = 14;
                                break;
                            case 256:
                                this.numberOfRounds = 16;
                                break;
                        }
                    }

                    public void SetKey(byte[] masterKey) {
                        if (masterKey.Length * 8 < keySize)
                            throw new Exception("Invalid masterKey size=" + masterKey.Length);
                        this.decRoundKeys = null;
                        this.encRoundKeys = null;
                        this.masterKey = (byte[])masterKey.Clone();
                    }

                    void SetupEncRoundKeys() {
                        if (this.keySize == 0)
                            throw new Exception("Invalid keySize");
                        if (this.masterKey == null)
                            throw new Exception("Invalid masterKey");
                        if (this.encRoundKeys == null)
                            this.encRoundKeys = new uint[4 * (this.numberOfRounds + 1)];
                        this.decRoundKeys = null;
                        DoEncKeySetup(this.masterKey, this.encRoundKeys, this.keySize);
                    }

                    void SetupDecRoundKeys() {
                        if (this.keySize == 0)
                            throw new Exception("Invalid keySize");
                        if (this.encRoundKeys == null)
                            if (this.masterKey == null)
                                throw new Exception("Invalid masterKey");
                            else
                                SetupEncRoundKeys();

                        this.decRoundKeys = (uint[])encRoundKeys.Clone();
                        DoDecKeySetup(this.masterKey, this.decRoundKeys, this.keySize);
                    }

                    public void SetupRoundKeys() {
                        SetupDecRoundKeys();
                    }

                    private static void DoCrypt(byte[] i, int ioffset, uint[] rk, int nr, byte[] o, int ooffset) {
                        uint t0, t1, t2, t3, j = 0;

                        t0 = ToInt(i[0 + ioffset], i[1 + ioffset], i[2 + ioffset], i[3 + ioffset]);
                        t1 = ToInt(i[4 + ioffset], i[5 + ioffset], i[6 + ioffset], i[7 + ioffset]);
                        t2 = ToInt(i[8 + ioffset], i[9 + ioffset], i[10 + ioffset], i[11 + ioffset]);
                        t3 = ToInt(i[12 + ioffset], i[13 + ioffset], i[14 + ioffset], i[15 + ioffset]);

                        for (int r = 1; r < nr / 2; r++) {
                            t0 ^= rk[j++]; t1 ^= rk[j++]; t2 ^= rk[j++]; t3 ^= rk[j++];
                            t0 = TS1[(t0 >> 24) & 0xff] ^ TS2[(t0 >> 16) & 0xff] ^ TX1[(t0 >> 8) & 0xff] ^ TX2[t0 & 0xff];
                            t1 = TS1[(t1 >> 24) & 0xff] ^ TS2[(t1 >> 16) & 0xff] ^ TX1[(t1 >> 8) & 0xff] ^ TX2[t1 & 0xff];
                            t2 = TS1[(t2 >> 24) & 0xff] ^ TS2[(t2 >> 16) & 0xff] ^ TX1[(t2 >> 8) & 0xff] ^ TX2[t2 & 0xff];
                            t3 = TS1[(t3 >> 24) & 0xff] ^ TS2[(t3 >> 16) & 0xff] ^ TX1[(t3 >> 8) & 0xff] ^ TX2[t3 & 0xff];
                            t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
                            t1 = Badc(t1); t2 = Cdab(t2); t3 = Dcba(t3);
                            t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;

                            t0 ^= rk[j++]; t1 ^= rk[j++]; t2 ^= rk[j++]; t3 ^= rk[j++];
                            t0 = TX1[(t0 >> 24) & 0xff] ^ TX2[(t0 >> 16) & 0xff] ^ TS1[(t0 >> 8) & 0xff] ^ TS2[t0 & 0xff];
                            t1 = TX1[(t1 >> 24) & 0xff] ^ TX2[(t1 >> 16) & 0xff] ^ TS1[(t1 >> 8) & 0xff] ^ TS2[t1 & 0xff];
                            t2 = TX1[(t2 >> 24) & 0xff] ^ TX2[(t2 >> 16) & 0xff] ^ TS1[(t2 >> 8) & 0xff] ^ TS2[t2 & 0xff];
                            t3 = TX1[(t3 >> 24) & 0xff] ^ TX2[(t3 >> 16) & 0xff] ^ TS1[(t3 >> 8) & 0xff] ^ TS2[t3 & 0xff];
                            t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
                            t3 = Badc(t3); t0 = Cdab(t0); t1 = Dcba(t1);
                            t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
                        }
                        t0 ^= rk[j++]; t1 ^= rk[j++]; t2 ^= rk[j++]; t3 ^= rk[j++];
                        t0 = TS1[(t0 >> 24) & 0xff] ^ TS2[(t0 >> 16) & 0xff] ^ TX1[(t0 >> 8) & 0xff] ^ TX2[t0 & 0xff];
                        t1 = TS1[(t1 >> 24) & 0xff] ^ TS2[(t1 >> 16) & 0xff] ^ TX1[(t1 >> 8) & 0xff] ^ TX2[t1 & 0xff];
                        t2 = TS1[(t2 >> 24) & 0xff] ^ TS2[(t2 >> 16) & 0xff] ^ TX1[(t2 >> 8) & 0xff] ^ TX2[t2 & 0xff];
                        t3 = TS1[(t3 >> 24) & 0xff] ^ TS2[(t3 >> 16) & 0xff] ^ TX1[(t3 >> 8) & 0xff] ^ TX2[t3 & 0xff];
                        t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
                        t1 = Badc(t1); t2 = Cdab(t2); t3 = Dcba(t3);
                        t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;

                        t0 ^= rk[j++]; t1 ^= rk[j++]; t2 ^= rk[j++]; t3 ^= rk[j++];
                        o[0 + ooffset] = (byte)(X1[0xff & (t0 >> 24)] ^ (rk[j] >> 24));
                        o[1 + ooffset] = (byte)(X2[0xff & (t0 >> 16)] ^ (rk[j] >> 16));
                        o[2 + ooffset] = (byte)(S1[0xff & (t0 >> 8)] ^ (rk[j] >> 8));
                        o[3 + ooffset] = (byte)(S2[0xff & (t0)] ^ (rk[j]));
                        o[4 + ooffset] = (byte)(X1[0xff & (t1 >> 24)] ^ (rk[j + 1] >> 24));
                        o[5 + ooffset] = (byte)(X2[0xff & (t1 >> 16)] ^ (rk[j + 1] >> 16));
                        o[6 + ooffset] = (byte)(S1[0xff & (t1 >> 8)] ^ (rk[j + 1] >> 8));
                        o[7 + ooffset] = (byte)(S2[0xff & (t1)] ^ (rk[j + 1]));
                        o[8 + ooffset] = (byte)(X1[0xff & (t2 >> 24)] ^ (rk[j + 2] >> 24));
                        o[9 + ooffset] = (byte)(X2[0xff & (t2 >> 16)] ^ (rk[j + 2] >> 16));
                        o[10 + ooffset] = (byte)(S1[0xff & (t2 >> 8)] ^ (rk[j + 2] >> 8));
                        o[11 + ooffset] = (byte)(S2[0xff & (t2)] ^ (rk[j + 2]));
                        o[12 + ooffset] = (byte)(X1[0xff & (t3 >> 24)] ^ (rk[j + 3] >> 24));
                        o[13 + ooffset] = (byte)(X2[0xff & (t3 >> 16)] ^ (rk[j + 3] >> 16));
                        o[14 + ooffset] = (byte)(S1[0xff & (t3 >> 8)] ^ (rk[j + 3] >> 8));
                        o[15 + ooffset] = (byte)(S2[0xff & (t3)] ^ (rk[j + 3]));
                    }

                    public void Encrypt(byte[] i, int ioffset, byte[] o, int ooffset) {
                        if (this.keySize == 0)
                            throw new Exception("Invalid keySize");
                        if (this.encRoundKeys == null)
                            if (this.masterKey == null)
                                throw new Exception("Invalid masterKey");
                            else
                                SetupEncRoundKeys();
                        DoCrypt(i, ioffset, this.encRoundKeys, this.numberOfRounds, o, ooffset);
                    }

                    public byte[] Encrypt(byte[] i, int ioffset) {
                        byte[] o = new byte[16];
                        this.Encrypt(i, ioffset, o, 0);
                        return o;
                    }

                    public void Decrypt(byte[] i, int ioffset, byte[] o, int ooffset) {
                        if (this.keySize == 0)
                            throw new Exception("Invalid keySize");
                        if (this.decRoundKeys == null)
                            if (this.masterKey == null)
                                throw new Exception("Invalid masterKey");
                            else
                                SetupDecRoundKeys();
                        DoCrypt(i, ioffset, this.decRoundKeys, this.numberOfRounds, o, ooffset);
                    }

                    public byte[] Decrypt(byte[] i, int ioffset) {
                        byte[] o = new byte[16];
                        this.Decrypt(i, ioffset, o, 0);
                        return o;
                    }

                    private static void DoEncKeySetup(byte[] mk, uint[] rk, int keyBits) {
                        uint t0, t1, t2, t3;
                        int q, j = 0;
                        uint[] w0 = new uint[4];
                        uint[] w1 = new uint[4];
                        uint[] w2 = new uint[4];
                        uint[] w3 = new uint[4];

                        w0[0] = ToInt(mk[0], mk[1], mk[2], mk[3]);
                        w0[1] = ToInt(mk[4], mk[5], mk[6], mk[7]);
                        w0[2] = ToInt(mk[8], mk[9], mk[10], mk[11]);
                        w0[3] = ToInt(mk[12], mk[13], mk[14], mk[15]);

                        q = (keyBits - 128) / 64;
                        t0 = w0[0] ^ KRK[q, 0]; t1 = w0[1] ^ KRK[q, 1];
                        t2 = w0[2] ^ KRK[q, 2]; t3 = w0[3] ^ KRK[q, 3];
                        t0 = TS1[(t0 >> 24) & 0xff] ^ TS2[(t0 >> 16) & 0xff] ^ TX1[(t0 >> 8) & 0xff] ^ TX2[t0 & 0xff];
                        t1 = TS1[(t1 >> 24) & 0xff] ^ TS2[(t1 >> 16) & 0xff] ^ TX1[(t1 >> 8) & 0xff] ^ TX2[t1 & 0xff];
                        t2 = TS1[(t2 >> 24) & 0xff] ^ TS2[(t2 >> 16) & 0xff] ^ TX1[(t2 >> 8) & 0xff] ^ TX2[t2 & 0xff];
                        t3 = TS1[(t3 >> 24) & 0xff] ^ TS2[(t3 >> 16) & 0xff] ^ TX1[(t3 >> 8) & 0xff] ^ TX2[t3 & 0xff];
                        t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
                        t1 = Badc(t1); t2 = Cdab(t2); t3 = Dcba(t3);
                        t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;

                        if (keyBits > 128) {
                            w1[0] = ToInt(mk[16], mk[17], mk[18], mk[19]);
                            w1[1] = ToInt(mk[20], mk[21], mk[22], mk[23]);
                            if (keyBits > 192) {
                                w1[2] = ToInt(mk[24], mk[25], mk[26], mk[27]);
                                w1[3] = ToInt(mk[28], mk[29], mk[30], mk[31]);
                            } else {
                                w1[2] = w1[3] = 0;
                            }
                        } else {
                            w1[0] = w1[1] = w1[2] = w1[3] = 0;
                        }
                        w1[0] ^= t0; w1[1] ^= t1; w1[2] ^= t2; w1[3] ^= t3;
                        t0 = w1[0]; t1 = w1[1]; t2 = w1[2]; t3 = w1[3];

                        q = (q == 2) ? 0 : (q + 1);
                        t0 ^= KRK[q, 0]; t1 ^= KRK[q, 1]; t2 ^= KRK[q, 2]; t3 ^= KRK[q, 3];
                        t0 = TX1[(t0 >> 24) & 0xff] ^ TX2[(t0 >> 16) & 0xff] ^ TS1[(t0 >> 8) & 0xff] ^ TS2[t0 & 0xff];
                        t1 = TX1[(t1 >> 24) & 0xff] ^ TX2[(t1 >> 16) & 0xff] ^ TS1[(t1 >> 8) & 0xff] ^ TS2[t1 & 0xff];
                        t2 = TX1[(t2 >> 24) & 0xff] ^ TX2[(t2 >> 16) & 0xff] ^ TS1[(t2 >> 8) & 0xff] ^ TS2[t2 & 0xff];
                        t3 = TX1[(t3 >> 24) & 0xff] ^ TX2[(t3 >> 16) & 0xff] ^ TS1[(t3 >> 8) & 0xff] ^ TS2[t3 & 0xff];
                        t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
                        t3 = Badc(t3); t0 = Cdab(t0); t1 = Dcba(t1);
                        t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
                        t0 ^= w0[0]; t1 ^= w0[1]; t2 ^= w0[2]; t3 ^= w0[3];
                        w2[0] = t0; w2[1] = t1; w2[2] = t2; w2[3] = t3;

                        q = (q == 2) ? 0 : (q + 1);
                        t0 ^= KRK[q, 0]; t1 ^= KRK[q, 1]; t2 ^= KRK[q, 2]; t3 ^= KRK[q, 3];
                        t0 = TS1[(t0 >> 24) & 0xff] ^ TS2[(t0 >> 16) & 0xff] ^ TX1[(t0 >> 8) & 0xff] ^ TX2[t0 & 0xff];
                        t1 = TS1[(t1 >> 24) & 0xff] ^ TS2[(t1 >> 16) & 0xff] ^ TX1[(t1 >> 8) & 0xff] ^ TX2[t1 & 0xff];
                        t2 = TS1[(t2 >> 24) & 0xff] ^ TS2[(t2 >> 16) & 0xff] ^ TX1[(t2 >> 8) & 0xff] ^ TX2[t2 & 0xff];
                        t3 = TS1[(t3 >> 24) & 0xff] ^ TS2[(t3 >> 16) & 0xff] ^ TX1[(t3 >> 8) & 0xff] ^ TX2[t3 & 0xff];
                        t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
                        t1 = Badc(t1); t2 = Cdab(t2); t3 = Dcba(t3);
                        t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
                        w3[0] = t0 ^ w1[0]; w3[1] = t1 ^ w1[1]; w3[2] = t2 ^ w1[2]; w3[3] = t3 ^ w1[3];

                        Gsrk(w0, w1, 19, rk, j); j += 4;
                        Gsrk(w1, w2, 19, rk, j); j += 4;
                        Gsrk(w2, w3, 19, rk, j); j += 4;
                        Gsrk(w3, w0, 19, rk, j); j += 4;
                        Gsrk(w0, w1, 31, rk, j); j += 4;
                        Gsrk(w1, w2, 31, rk, j); j += 4;
                        Gsrk(w2, w3, 31, rk, j); j += 4;
                        Gsrk(w3, w0, 31, rk, j); j += 4;
                        Gsrk(w0, w1, 67, rk, j); j += 4;
                        Gsrk(w1, w2, 67, rk, j); j += 4;
                        Gsrk(w2, w3, 67, rk, j); j += 4;
                        Gsrk(w3, w0, 67, rk, j); j += 4;
                        Gsrk(w0, w1, 97, rk, j); j += 4;
                        if (keyBits > 128) {
                            Gsrk(w1, w2, 97, rk, j); j += 4;
                            Gsrk(w2, w3, 97, rk, j); j += 4;
                        }
                        if (keyBits > 192) {
                            Gsrk(w3, w0, 97, rk, j); j += 4;
                            Gsrk(w0, w1, 109, rk, j);
                        }
                    }

                    /**
                      * Main bulk of the decryption key setup method.  Here we assume that
                     * the int array rk already contains the encryption round keys.
                     * @param mk the master key
                     * @param rk the array which contains the encryption round keys at the
                     * beginning of the method execution.  At the end of method execution
                     * this will hold the decryption round keys.
                     * @param keyBits the length of the master key
                     * @return
                     */
                    private static void DoDecKeySetup(byte[] mk, uint[] rk, int keyBits) {
                        int a = 0, z;
                        uint[] t = new uint[4];

                        z = 32 + keyBits / 8;
                        SwapBlocks(rk, 0, z);
                        a += 4; z -= 4;

                        for (; a < z; a += 4, z -= 4)
                            SwapAndDiffuse(rk, a, z, t);
                        Diff(rk, a, t, 0);
                        rk[a] = t[0]; rk[a + 1] = t[1]; rk[a + 2] = t[2]; rk[a + 3] = t[3];
                    }

                    private static uint ToInt(byte b0, byte b1, byte b2, byte b3) {
                        return (uint)((b0 & 0xff) << 24 ^ (b1 & 0xff) << 16 ^ (b2 & 0xff) << 8 ^ b3 & 0xff);
                    }

                    private static void ToByteArray(uint i, byte[] b, int offset) {
                        b[offset] = (byte)(i >> 24);
                        b[offset + 1] = (byte)(i >> 16);
                        b[offset + 2] = (byte)(i >> 8);
                        b[offset + 3] = (byte)(i);
                    }

                    private static uint M(uint t) {
                        return 0x00010101 * ((t >> 24) & 0xff) ^ 0x01000101 * ((t >> 16) & 0xff) ^
                            0x01010001 * ((t >> 8) & 0xff) ^ 0x01010100 * (t & 0xff);
                    }

                    //  private static final int ms(int t) {
                    //    return TS1[(t>>>24)&0xff]^TS2[(t>>>16)&0xff]^TX1[(t>>>8)&0xff]^TX2[t&0xff];
                    //  }
                    //  private static final int mx(int t) {
                    //    return TX1[(t>>>24)&0xff]^TX2[(t>>>16)&0xff]^TS1[(t>>>8)&0xff]^TS2[t&0xff];
                    //  }
                    private static uint Badc(uint t) {
                        return ((t << 8) & 0xff00ff00) ^ ((t >> 8) & 0x00ff00ff);
                    }

                    private static uint Cdab(uint t) {
                        return ((t << 16) & 0xffff0000) ^ ((t >> 16) & 0x0000ffff);
                    }

                    private static uint Dcba(uint t) {
                        return (t & 0x000000ff) << 24 ^ (t & 0x0000ff00) << 8 ^ (t & 0x00ff0000) >> 8 ^ (t & 0xff000000) >> 24;
                    }

                    private static void Gsrk(uint[] x, uint[] y, int rot, uint[] rk, int offset) {
                        int q = 4 - (rot / 32), r = rot % 32, s = 32 - r;

                        rk[offset] = x[0] ^ y[(q) % 4] >> r ^ y[(q + 3) % 4] << s;
                        rk[offset + 1] = x[1] ^ y[(q + 1) % 4] >> r ^ y[(q) % 4] << s;
                        rk[offset + 2] = x[2] ^ y[(q + 2) % 4] >> r ^ y[(q + 1) % 4] << s;
                        rk[offset + 3] = x[3] ^ y[(q + 3) % 4] >> r ^ y[(q + 2) % 4] << s;
                    }

                    private static void Diff(uint[] i, int offset1, uint[] o, int offset2) {
                        uint t0, t1, t2, t3;

                        t0 = M(i[offset1]); t1 = M(i[offset1 + 1]); t2 = M(i[offset1 + 2]); t3 = M(i[offset1 + 3]);
                        t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
                        t1 = Badc(t1); t2 = Cdab(t2); t3 = Dcba(t3);
                        t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
                        o[offset2] = t0; o[offset2 + 1] = t1; o[offset2 + 2] = t2; o[offset2 + 3] = t3;
                    }

                    private static void SwapBlocks(uint[] arr, int offset1, int offset2) {
                        uint t;

                        for (int i = 0; i < 4; i++) {
                            t = arr[offset1 + i];
                            arr[offset1 + i] = arr[offset2 + i];
                            arr[offset2 + i] = t;
                        }
                    }

                    private static void SwapAndDiffuse(uint[] arr, int offset1, int offset2, uint[] tmp) {
                        Diff(arr, offset1, tmp, 0);
                        Diff(arr, offset2, arr, offset1);
                        arr[offset2] = tmp[0]; arr[offset2 + 1] = tmp[1];
                        arr[offset2 + 2] = tmp[2]; arr[offset2 + 3] = tmp[3];
                    }

                    private static void PrintBlock(TextWriter writer, byte[] b) {
                        for (int i = 0; i < 4; i++)
                            ByteToHex(writer, b[i]);
                        writer.Write(" ");
                        for (int i = 4; i < 8; i++)
                            ByteToHex(writer, b[i]);
                        writer.Write(" ");
                        for (int i = 8; i < 12; i++)
                            ByteToHex(writer, b[i]);
                        writer.Write(" ");
                        for (int i = 12; i < 16; i++)
                            ByteToHex(writer, b[i]);
                    }

                    private static void PrintSBox(TextWriter writer, byte[] box) {
                        for (int i = 0; i < 16; i++) {
                            for (int j = 0; j < 16; j++) {
                                ByteToHex(writer, box[16 * i + j]);
                                writer.Write(" ");
                            }
                            writer.WriteLine();
                        }
                    }

                    private static void ByteToHex(TextWriter writer, byte b) {
                        char[] buf = {
                HEX_DIGITS[(b >> 4) & 0x0F],
                HEX_DIGITS[ b       & 0x0F]
            };
                        writer.Write(buf);
                    }

                    private static void IntToHex(TextWriter writer, uint i) {
                        byte[] b = new byte[4];
                        ToByteArray(i, b, 0);
                        ByteToHex(writer, b[0]);
                        ByteToHex(writer, b[1]);
                        ByteToHex(writer, b[2]);
                        ByteToHex(writer, b[3]);
                    }

                    private static void PrintRoundKeys(TextWriter writer, uint[] roundKeys) {
                        for (int i = 0; i < roundKeys.Length;) {
                            writer.Write("* ");
                            IntToHex(writer, roundKeys[i++]); writer.Write(" ");
                            IntToHex(writer, roundKeys[i++]); writer.Write(" ");
                            IntToHex(writer, roundKeys[i++]); writer.Write(" ");
                            IntToHex(writer, roundKeys[i++]); writer.WriteLine(" ");
                        }
                    }

                    protected static byte[] MasterKey = new byte[16];
                    public static void setMasterKey(byte[] mk) => MasterKey = mk;
                    public static void ARIA_test() {
                        byte[] p = new byte[16];
                        byte[] c = new byte[16];

                        TextWriter writer = Console.Out;
                        ARIAEngine instance = new ARIAEngine(128);

                        for (int i = 0; i < 16; i++)
                            p[i] = 0;

                        p = new byte[16] { 0x32, 0x30, 0x39, 0x35, 0x32, 0x35, 0x30, 0x30, 0x30, 0x30, 0x31, 0x36, 0x37, 0x35, 0x39, 0x39 };

                        instance.SetKey(MasterKey);
                        instance.SetupRoundKeys();

                        instance.Encrypt(p, 0, c, 0);
                        instance.Decrypt(c, 0, p, 0);
                    }

                    public static byte[] getARIAEncrypt(byte[] data) {
                        byte[] p = new byte[16];
                        byte[] c = new byte[16];
                        ARIAEngine instance = new ARIAEngine(128);

                        for (int i = 0; i < 16; i++)
                            p[i] = 0x20;

                        Array.Copy(data, 0, p, 0, data.Length);

                        instance.SetKey(MasterKey);
                        instance.SetupRoundKeys();
                        instance.Encrypt(p, 0, c, 0);

                        return c;
                    }
                    public static byte[] getARIADecrypt(byte[] data) {
                        byte[] p = new byte[16];
                        byte[] c = new byte[16];
                        ARIAEngine instance = new ARIAEngine(128);

                        for (int i = 0; i < 16; i++)
                            c[i] = 0x20;

                        Array.Copy(data, 0, c, 0, data.Length);

                        instance.SetKey(MasterKey);
                        instance.SetupRoundKeys();
                        instance.Decrypt(c, 0, p, 0);

                        return p;
                    }
                }
            }
        }
    }
}
