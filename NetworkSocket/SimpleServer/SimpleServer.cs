using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer
{
    public class SimpleServer
    {
        private Socket listenSocket;

        private SocketAsyncEventArgs acceptArg = new SocketAsyncEventArgs();
        public void StartListen(int port)
        {

        }

        public void StartListen(IPEndPoint localEndPoint)
        {
            this.listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.listenSocket.Bind(localEndPoint);
            this.listenSocket.Listen(100);//挂起连接的最大长度

            this.acceptArg = new SocketAsyncEventArgs();
            this.acceptArg.Completed += (sender, e) =>  this.AcceptArgCompleted(e);
            this.AcceptSession(this.acceptArg);

            
        }
    
        private void AcceptSession(SocketAsyncEventArgs arg)
        {
            if(this.listenSocket != null)
            {
                arg.AcceptSocket = null;
                if(this.listenSocket.AcceptAsync(arg) == false)
                {
                    this.AcceptArgCompleted(arg);
                }
            }
        }

        private void AcceptArgCompleted(SocketAsyncEventArgs arg)
        {
            var socket = arg.AcceptSocket;
            if(arg.SocketError == SocketError.Success)
            {
               // var session = this.
            }
            else
            {
                //InterLocked
            }

            this.AcceptSession(arg);
        }

    }
}
