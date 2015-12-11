<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleClient
{
    class EasyClient
    {
        /// <summary>
        /// 同步发送
        /// </summary>
        public void SendSync()
        {
            int port = 6000;
            string host = "127.0.0.1";
            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);

            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(ipe);

            string sendStr = "send to server: hello,ni hao";
            byte[] sendBytes = Encoding.ASCII.GetBytes(sendStr);
            clientSocket.Send(sendBytes);

            string recStr = "";
            byte[] recBytes = new byte[4096];
            int bytes = clientSocket.Receive(recBytes, recBytes.Length, 0);
            recStr += Encoding.ASCII.GetString(recBytes, 0, bytes);
            Console.WriteLine(recStr);

            clientSocket.Close();
        }

        private Socket clientSocket = null;
        public void Connect(IPAddress ip,int port)
        {
            this.clientSocket.BeginConnect(ip, port, new AsyncCallback(ConnectCallback), this.clientSocket);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                handler.EndConnect(ar);
            }
            catch (SocketException ex)
            {

                throw;
            }
        }

        public void SendAsync(string data)
        {
            Send(System.Text.Encoding.UTF8.GetBytes(data));
        }

        private void Send(byte[] byteData)
        {
            try
            {
                int length = byteData.Length;
                byte[] head = BitConverter.GetBytes(length);
                byte[] data = new byte[head.Length + byteData.Length];
                Array.Copy(head, data, head.Length);
                Array.Copy(byteData, 0, data, head.Length, byteData.Length);
                this.clientSocket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), this.clientSocket);
            }
            catch (SocketException ex)
            {

                throw;
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                handler.EndSend(ar);
            }
            catch (SocketException ex)
            {

                throw;
            }
        }

        private byte[] MsgBuffer;
        public void ReceiveData()
        {
            clientSocket.BeginReceive(MsgBuffer, 0, MsgBuffer.Length, 0, new AsyncCallback(ReceiveCallback), null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int REnd = clientSocket.EndReceive(ar);
                if(REnd > 0)
                {
                    byte[] data = new byte[REnd];
                    data.CopyTo(null,)
                    Array.Copy(MsgBuffer, 0, data, 0, REnd);

                    //在此处可以对data进行按需处理
                    clientSocket.BeginReceive(MsgBuffer, 0, MsgBuffer.Length, 0, new AsyncCallback(ReceiveCallback), null);
                }
                else
                {
                    Dispose();
                }
            }
            catch (SocketException ex)
            {

                throw;
            }
        }

        private void Dispose()
        {
            try
            {
                this.clientSocket.Shutdown(SocketShutdown.Both);
                this.clientSocket.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private HashTable DataTable = new HashTable();

        /// <summary>
        /// http://www.cnblogs.com/ysyn/p/3399351.html
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ip"></param>
        public void DataArrial(byte[] data,string ip)
        {
            try
            {
                if(data.Length < 12)
                {
                    lock (DataTable)
                    {
                        if(DataTable.Contains(ip))
                        {
                            DataTable[ip] = data;
                            return;
                        }
                    }
                }

                if (data[0] != 0x1F || data[1] != 0xF1) 
                {
                    if(DataTable.Contains(ip))
                    {
                        if(DataTable != null)
                        {
                            byte[] oldData = (byte[])DataTable[ip];//取出粘包数据
                            if(oldData[0] != 0x1F || oldData[1] != 0xF1)
                            {
                                return;
                            }

                            byte[] newData = new byte[data.Length + oldData.Length];
                            Array.Copy(oldData, 0, newData, 0, oldData.Length);
                            Array.Copy(data, 0, newData, oldData.Length, data.Length);//组成新数组,先到的排前，后到的排后

                            lock (DataTable)
                            {
                                DataTable[ip] = null;

                            }
                            DataArrial(newData, ip);
                            return;
                        }
                    }
                    return;
                }

                int revDataLength = data[2];
                int revCount = data.Length;//
                if(revCount > revDataLength)//如果接受的长度大于发送的数据长度，
                {
                    byte[] otherData = new byte[revCount - revDataLength];
                    data.CopyTo(otherData, revCount - 1);
                    Array.Copy(data, revDataLength, otherData, 0, otherData.Length);
                    //data = (byte[])
                    DataArrial(otherData, ip);
                }
                else if (revCount < revDataLength)
                {
                    if(DataTable.Contains(ip))
                    {
                        DataTable[ip] = data;//更新当前粘包数据
                        return;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private Array Redim(Array origArray,Int32 desizedSize)
        {
            Type t = origArray.GetType().GetElementType();
            //
            Array newArray = Array.CreateInstance(t, desizedSize);
            Array.Copy(origArray, 0, newArray, 0, Math.Min(origArray.Length, desizedSize));
            return newArray;
        }
    }
}
=======
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleClient
{
    class EasyClient
    {
        /// <summary>
        /// 同步发送
        /// </summary>
        public void SendSync()
        {
            int port = 6000;
            string host = "127.0.0.1";
            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);

            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(ipe);

            string sendStr = "send to server: hello,ni hao";
            byte[] sendBytes = Encoding.ASCII.GetBytes(sendStr);
            clientSocket.Send(sendBytes);

            string recStr = "";
            byte[] recBytes = new byte[4096];
            int bytes = clientSocket.Receive(recBytes, recBytes.Length, 0);
            recStr += Encoding.ASCII.GetString(recBytes, 0, bytes);
            Console.WriteLine(recStr);

            clientSocket.Close();
        }

        private Socket clientSocket = null;
        public void Connect(IPAddress ip,int port)
        {
            this.clientSocket.BeginConnect(ip, port, new AsyncCallback(ConnectCallback), this.clientSocket);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                handler.EndConnect(ar);
            }
            catch (SocketException ex)
            {

                throw;
            }
        }

        public void SendAsync(string data)
        {
            Send(System.Text.Encoding.UTF8.GetBytes(data));
        }

        private void Send(byte[] byteData)
        {
            try
            {
                int length = byteData.Length;
                byte[] head = BitConverter.GetBytes(length);
                byte[] data = new byte[head.Length + byteData.Length];
                Array.Copy(head, data, head.Length);
                Array.Copy(byteData, 0, data, head.Length, byteData.Length);
                this.clientSocket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), this.clientSocket);
            }
            catch (SocketException ex)
            {

                throw;
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                handler.EndSend(ar);
            }
            catch (SocketException ex)
            {

                throw;
            }
        }

        private byte[] MsgBuffer;
        public void ReceiveData()
        {
            clientSocket.BeginReceive(MsgBuffer, 0, MsgBuffer.Length, 0, new AsyncCallback(ReceiveCallback), null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int REnd = clientSocket.EndReceive(ar);
                if(REnd > 0)
                {
                    byte[] data = new byte[REnd];
                    //data.CopyTo(null,)
                    Array.Copy(MsgBuffer, 0, data, 0, REnd);

                    //在此处可以对data进行按需处理
                    clientSocket.BeginReceive(MsgBuffer, 0, MsgBuffer.Length, 0, new AsyncCallback(ReceiveCallback), null);
                }
                else
                {
                    Dispose();
                }
            }
            catch (SocketException ex)
            {

                throw;
            }
        }

        private void Dispose()
        {
            try
            {
                this.clientSocket.Shutdown(SocketShutdown.Both);
                this.clientSocket.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private Hashtable DataTable = new Hashtable();

        /// <summary>
        /// http://www.cnblogs.com/ysyn/p/3399351.html
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ip"></param>
        public void DataArrial(byte[] data,string ip)
        {
            try
            {
                if(data.Length < 12)
                {
                    lock (DataTable)
                    {
                        if(DataTable.Contains(ip))
                        {
                            DataTable[ip] = data;
                            return;
                        }
                    }
                }

                if (data[0] != 0x1F || data[1] != 0xF1) 
                {
                    if(DataTable.Contains(ip))
                    {
                        if(DataTable != null)
                        {
                            byte[] oldData = (byte[])DataTable[ip];//取出粘包数据
                            if(oldData[0] != 0x1F || oldData[1] != 0xF1)
                            {
                                return;
                            }

                            byte[] newData = new byte[data.Length + oldData.Length];
                            Array.Copy(oldData, 0, newData, 0, oldData.Length);
                            Array.Copy(data, 0, newData, oldData.Length, data.Length);//组成新数组,先到的排前，后到的排后

                            lock (DataTable)
                            {
                                DataTable[ip] = null;

                            }
                            DataArrial(newData, ip);
                            return;
                        }
                    }
                    return;
                }

                int revDataLength = data[2];
                int revCount = data.Length;//
                if(revCount > revDataLength)//如果接受的长度大于发送的数据长度，
                {
                    byte[] otherData = new byte[revCount - revDataLength];
                    data.CopyTo(otherData, revCount - 1);
                    Array.Copy(data, revDataLength, otherData, 0, otherData.Length);
                    //data = (byte[])
                    DataArrial(otherData, ip);
                }
                else if (revCount < revDataLength)
                {
                    if(DataTable.Contains(ip))
                    {
                        DataTable[ip] = data;//更新当前粘包数据
                        return;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private Array Redim(Array origArray,Int32 desizedSize)
        {
            Type t = origArray.GetType().GetElementType();
            //
            Array newArray = Array.CreateInstance(t, desizedSize);
            Array.Copy(origArray, 0, newArray, 0, Math.Min(origArray.Length, desizedSize));
            return newArray;
        }
    }
}
>>>>>>> b238163ec6e714b9bfc2e090e7fb49dc84320a94
