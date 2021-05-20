using System.Net.Sockets;

namespace socket_server.Object
{
    public class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 2048;
        public byte[] buffer = new byte[BufferSize];
        //public StringBuilder sb = new StringBuilder();
        public bool thru = false;
        public string[,] detail = new string[10, 2]
        {
            { "Addr", "" },
            { "Type", "" },
            { "PlaceID", "" },
            { "DeviceID", "" },
            { "", "" },
            { "", "" },
            { "", "" },
            { "", "" },
            { "", "" },
            { "", "" }
        };
    }

    public class Detail
    {
        public enum TYPE
        {
            Addr, Type, PlaceID, DeviceID
        }
        public static void set(StateObject state, TYPE type, string content) {
            switch (type) {
                case TYPE.Addr:
                    state.detail[0, 1] = content;
                    break;
                case TYPE.Type:
                    state.detail[1, 1] = content;
                    break;
                case TYPE.PlaceID:
                    state.detail[2, 1] = content;
                    break;
                case TYPE.DeviceID:
                    state.detail[3, 1] = content;
                    break;
                default:
                    return;
            }
        }
        public static string get(StateObject state, TYPE type) {
            switch (type) {
                case TYPE.Addr:
                    return state.detail[0, 1];
                case TYPE.Type:
                    return state.detail[1, 1];
                case TYPE.PlaceID:
                    return state.detail[2, 1];
                case TYPE.DeviceID:
                    return state.detail[3, 1];
                default:
                    return null;
            }
        }
    }
}
