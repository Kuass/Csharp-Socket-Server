using MySql.Data.MySqlClient;
using socket_server.Object;
using System;
using System.Data;

namespace socket_server
{
    public class Database
    {
        protected static MySqlConnection dbconn;
        protected static MySqlCommand dbcmd;
        public static void startDatabase() {
            dbconn = new MySqlConnection("Server=***;database=test;charset=utf8");
            dbconn.Open();
        }
        public static void checkDatabase() {
            ConnectionState state = dbconn.State;
            if (state != ConnectionState.Open) {
                if (Server.DebugLevel > 2) Server.print(2, "The database is closed and has been reopened.");
                dbconn.Close();
                dbconn.Open();
            }
        }
        public class Charger
        {
            public class Session
            {
                public static void Join(StateObject state) {
                    checkDatabase();
                    string query = string.Format("INSERT INTO session(type, placeid, deviceid, addr, cert) VALUES({0},'{1}',{2},{3},'{4}',0)",
                                   Detail.get(state, Detail.TYPE.Type), Detail.get(state, Detail.TYPE.PlaceID), Detail.get(state, Detail.TYPE.DeviceID), Detail.get(state, Detail.TYPE.Addr));
                    dbcmd = new MySqlCommand(query, dbconn);
                    try {
                        if (dbcmd.ExecuteNonQuery() == 1) {
                            if (Server.DebugLevel > 2) Server.print(2, "The connection was successfully processed by the database session.");
                        } else Server.print(1, "The connection could not be included in the database session.. " + query);
                    } catch (Exception e) {
                        Server.print(1, "The connection could not be included in the database session.. " + e + "\n" + query);
                    }
                }

                public static void Leave(StateObject state) {
                    checkDatabase();
                    string query = string.Format("DELETE FROM session WHERE type={0} AND placeid='{1}' AND chargerid={2} AND addr='{4}'",
                        Detail.get(state, Detail.TYPE.Type), Detail.get(state, Detail.TYPE.PlaceID), Detail.get(state, Detail.TYPE.DeviceID), Detail.get(state, Detail.TYPE.Addr));
                    dbcmd = new MySqlCommand(query, dbconn);
                    try {
                        if (dbcmd.ExecuteNonQuery() == 1) {
                            if (Server.DebugLevel > 2) Server.print(2, "The session for the connection has been successfully deleted.");
                        } else Server.print(1, "Failed to delete session for this connection.. " + query);
                    } catch (Exception e) {
                        Server.print(1, "Failed to delete session for this connection.. " + e + "\n" + query);
                    }
                }
            }

            public class Record
            {
                public static void LeaveDataRaw(bool r_send, string[] data, byte[] r_packet, bool r_resend) {
                    checkDatabase();
                    int resend = (r_resend) ? 1 : 0;
                    int send = (r_send) ? 1 : 0;
                    string packet = Convertion.ByteArrayToHexString(r_packet);
                    string query = string.Format("INSERT INTO h_packet(type, placeid, deviceid, addr, ins, DATA, resend) VALUES({0},'{1}',{2},{3},'{4}','{5}','{6}',{7})",
                                   send, data[0], data[1], data[2], data[3], packet, resend);
                    dbcmd = new MySqlCommand(query, dbconn);
                    try {
                        if (dbcmd.ExecuteNonQuery() == 1) {
                            if (Server.DebugLevel > 2) Server.print(2, "Communication history successfully left in the database.");
                        } else Server.print(1, "LeaveDataRaw could not be reflected in the database.. " + query);
                    } catch (Exception e) {
                        Server.print(1, "LeaveDataRaw could not be reflected in the database.. " + e + "\n" + query);
                    }
                }
            }
        }
    }
}
