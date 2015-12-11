//<<<<<<< HEAD
//﻿using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Net.Sockets;
//using System.Net;

//namespace SimpleClient
//{
//    public class Client
//    {
//        private static byte[] buf = new byte[1024];
//        static void Main(string[] args)
//        {
//            Console.Write("Enter your name:");
//            string name = Console.ReadLine();
//            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
//                ProtocolType.Tcp);

//            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8064));
//            Console.WriteLine("Connected to server,enter $q to quit");
//            name = "{<" + name.Trim() +">}";
//            byte[] nameBuf = Encoding.UTF8.GetBytes(name);
//            client.BeginSend(nameBuf, 0, nameBuf.Length, SocketFlags.None, null, null);
//            client.BeginReceive(buf, 0, buf.Length, SocketFlags.None, new AsyncCallback(Receive), client);
//            while (true)
//            {
//                string msg = Console.ReadLine();
//                if (msg == "$q")
//                {
//                    client.Close();
//                    break;
//                }

//                byte[] output = Encoding.UTF8.GetBytes(msg);
//                client.BeginSend(output, 0, output.Length, SocketFlags.None, null, null);
//            }
//            Console.Write("Disconnected . Press any key to exit...");
//            Console.ReadKey();

//        }

//        private static void Receive(IAsyncResult result)
//        {
//            try
//            {
//                Socket client = result.AsyncState as Socket;
//                int Length = client.EndReceive(result);
//                string msg = Encoding.UTF8.GetString(buf, 0, Length);
//                Console.WriteLine(msg);
//                client.BeginReceive(buf, 0, buf.Length, SocketFlags.None, new AsyncCallback(Receive), client);
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }
//    }
//}
//=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace SimpleClient
{
    public class Client
    {
        private static byte[] buf = new byte[1024];
        static void Main(string[] args)
        {
            Console.Write("Enter your name:");
            string name = Console.ReadLine();
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);

            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080));
            Console.WriteLine("Connected to server,enter $q to quit");
            name = "{<" + name.Trim() +">}";
            byte[] nameBuf = Encoding.UTF8.GetBytes(name);
            client.BeginSend(nameBuf, 0, nameBuf.Length, SocketFlags.None, null, null);
            client.BeginReceive(buf, 0, buf.Length, SocketFlags.None, new AsyncCallback(Receive), client);
            while (true)
            {
                string msg = Console.ReadLine();
                if (msg == "$q")
                {
                    client.Close();
                    break;
                }

                byte[] output = Encoding.UTF8.GetBytes(msg);
                client.BeginSend(output, 0, output.Length, SocketFlags.None, null, null);
            }
            Console.Write("Disconnected . Press any key to exit...");
            Console.ReadKey();

        }

        private static void Receive(IAsyncResult result)
        {
            try
            {
                Socket client = result.AsyncState as Socket;
                int Length = client.EndReceive(result);
                string msg = Encoding.UTF8.GetString(buf, 0, Length);
                Console.WriteLine(msg);
                client.BeginReceive(buf, 0, buf.Length, SocketFlags.None, new AsyncCallback(Receive), client);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
//>>>>>>> fa1c9a487073a16a5293b131666f8da981b2ba0f
