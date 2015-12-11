using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;

namespace SimpleSocket
{

    class Program
    {
        static void Main(string[] args)
        {
            SocketServer socketServer = new SocketServer();
            socketServer.Run(8080);
        }
    }

    public class SocketServer
    {

        

        private Dictionary<Socket, ClientInfo> clientPool = new Dictionary<Socket, ClientInfo>();

        private List<SocketMessage> MsgPool = new List<SocketMessage>();

        /// <summary>
        /// 启动服务器，监听客户端请求
        /// </summary>
        /// <param name="port"></param>
        public void Run(int port)
        {
            Thread serverSocketThread = new Thread(
                () =>
                {
                    Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                        ProtocolType.Tcp);
                    server.Bind(new IPEndPoint(IPAddress.Any, port));
                    server.Listen(10);
                    server.BeginAccept(new AsyncCallback(Accept), server);
                
                }


                );

            serverSocketThread.Start();
            Console.WriteLine("Server is ready");
            Broadcast();

        }
        /// <summary>
        /// 处理客户端连接请求，成功后把客户端加入到clientPool中
        /// </summary>
        /// <param name="result"></param>
        private void Accept(IAsyncResult result)
        {
            Socket server = result.AsyncState as Socket;
            Socket client = server.EndAccept(result);
            try
            {
                server.BeginAccept(new AsyncCallback(Accept), server);
                byte[] buffer = new byte[1024];
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Receive), client);
                ClientInfo info = new ClientInfo();
                info.Id = client.RemoteEndPoint;
                info.handle = client.Handle;
                info.buffer = buffer;
                this.clientPool.Add(client, info);
                Console.WriteLine(string.Format("Client {0} connected",
                    client.RemoteEndPoint));
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error :\r\n\t" + ex.ToString());
            }
        }

        /// <summary>
        /// 处理客户端发来的消息，接受成功后加入到msgPool,等待广播
        /// </summary>
        /// <param name="result"></param>
        private void Receive(IAsyncResult result)
        {
            Socket client = result.AsyncState as Socket;
            if (client == null || !clientPool.ContainsKey(client))
                return;

            try
            {
                int length = client.EndReceive(result);
                byte[] buffer = clientPool[client].buffer;

                client.BeginReceive(buffer, 0, buffer.Length,SocketFlags.None,new AsyncCallback(Receive),client);

                string msg = Encoding.UTF8.GetString(buffer, 0, length);
                SocketMessage sm = new SocketMessage();
                sm.Client = clientPool[client];
                sm.Time = DateTime.Now;

                Regex reg = new Regex(@"{(.*?)>}");
                Match m = reg.Match(msg);
                if (m.Value != "")
                {
                    clientPool[client].NickName = Regex.Replace(m.Value, @"{<(.*?)>}", "$1");
                    sm.isLogin = true;
                    sm.Message = "login!";
                    Console.WriteLine("{0} login @ {1}", client.RemoteEndPoint, DateTime.Now);
                }
                else
                {
                    sm.isLogin = false;
                    sm.Message = msg;
                    Console.WriteLine("{0} @ {1}\r\n {2}", client.RemoteEndPoint, DateTime.Now, msg);
                }
                MsgPool.Add(sm);
            }
            catch (Exception)
            {

                client.Disconnect(true);
                Console.WriteLine("Client {0} disconnect",clientPool[client].Name);
                clientPool.Remove(client);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Broadcast()
        {
            Thread broadcast = new Thread(
                () => {

                    while (true)
                    {
                        if (MsgPool.Count > 0)
                        {
                            byte[] msg = PackageMessage(MsgPool[0]);
                            foreach (KeyValuePair<Socket,ClientInfo> cs in clientPool)
                            {
                                Socket client = cs.Key;
                                if (client.Connected)
                                {
                                    client.Send(msg, msg.Length, SocketFlags.None);
                                }
                            }
                            MsgPool.RemoveAt(0);
                        }
                    }
                }
                );
            broadcast.Start();
        }

        private byte[] PackageMessage(SocketMessage sm)
        {
            StringBuilder packagedMsg = new StringBuilder();
            if (!sm.isLogin)
            {
                packagedMsg.AppendFormat("{0} @ {1}:\r\n ", sm.Client.Name, sm.Time.ToShortTimeString());
                packagedMsg.Append(sm.Message);
            }
            else
            {
                packagedMsg.AppendFormat("{0} login @ {1}", sm.Client.Name, sm.Time.ToShortTimeString());


            }
            return Encoding.UTF8.GetBytes(packagedMsg.ToString());
        }
    }
}
