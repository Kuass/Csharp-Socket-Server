using socket_server.Object;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace socket_server
{
    public class Server
    {
        public static void ReadCallback(IAsyncResult ar) {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            try {
                int bytesRead = handler.EndReceive(ar);

                Detail.set(state, Detail.TYPE.Addr, state.workSocket.RemoteEndPoint.ToString());
                if (bytesRead > 0) {
                    if (!DataReceive.Get(state, bytesRead)) {
                        print(0, "Disconnect to " + Detail.get(state, Detail.TYPE.Addr) + " (Shutdown)");
                        state.thru = true;
                        Database.Charger.Session.Leave(state);
                        handler.Dispose();
                        return;
                    }

                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            } catch (Exception e) {
                try {
                    Database.Charger.Session.Leave(state);
                    print(0, "Disconnect to " + Detail.get(state, Detail.TYPE.Addr) + " (" + e.ToString() + ")"); state.thru = true; handler.Dispose();
                } catch (Exception e2) {
                    print(3, e2.ToString());
                    handler.Dispose();
                }
            }
        }
        public static void Send(Socket handler, byte[] data) {
            try {
                handler.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), handler);
            } catch (Exception e) {
                try {
                    print(1, handler.RemoteEndPoint.ToString() + " (" + e.ToString() + ")");
                } catch (Exception e2) {
                    print(3, e2.ToString());
                    handler.Dispose();
                }
            }
        }

        private static void SendCallback(IAsyncResult ar) {
            try {
                Socket handler = (Socket)ar.AsyncState;

                int bytesSent = handler.EndSend(ar);
                print(2, "Sent Success - " + bytesSent);
            } catch (Exception e) {
                print(1, e.ToString());
            }
        }
        public static void AcceptCallback(IAsyncResult ar) {
            // 메인 스레드가 계속해서 신호를 출력합니다.
            allDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            print(0, "New Connection to " + state.workSocket.RemoteEndPoint.ToString());

            Thread.Sleep(30000);
            if (!state.thru) {
                print(0, "Disconnect to " + state.workSocket.RemoteEndPoint.ToString() + " TimeOut"); handler.Close(); state.workSocket.Close();
            }
        }

        // 스레드 신호.
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        static void startServer() {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8510);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEndPoint);
            listener.Listen(100);
            print(0, "Ready to Mog-Server");
            print(0, "Waiting for Database Signal...");
            Database.startDatabase();
            print(0, "Database is Ready.");
            print(0, "Waiting for a connection...");
            while (true) {
                allDone.Reset();
                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                allDone.WaitOne();
            }
        }

        public static void print(int type, string content) {
            if (type == 0)
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " [INFO] " + content);
            else if (type == 1)
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " [ERROR] " + content);
            else if (type == 2)
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " [DEBUG] " + content);
            else if (type == 3) {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " [FATAL] " + content);
                Console.ResetColor();
            }
        }

        public const int DebugLevel = 3;
        static void Main(string[] args) {
            Server.startServer();
        }
    }
}
