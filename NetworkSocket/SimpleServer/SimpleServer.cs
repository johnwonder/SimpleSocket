using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer
{
    public class SimpleServer<T>
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

            //处理后继续接收
            this.AcceptSession(arg);
        }

        /// <summary>
        /// 空闲客户端池
        /// </summary>
        private FreeSessionStack<T> freeSessionStack = new FreeSessionStack<T>();
        /// <summary>
        /// 从空闲池中取出或创建Session会话对象
        /// </summary>
        /// <returns></returns>
        private T TakeOrCreateSession()
        {
            var session = this.freeSessionStack.Take();
            if (session != null)
                return session;

            try
            {
                session = this.OnCreateSession();
                if(session != null)
                {

                }
            }
            catch (Exception)
            {

                throw;
            }
            return session;
        }

        protected abstract T OnCreateSession();

        private void InitSession(T session,Socket socket)
        {
            //session.ReciveHandler =>(buffer) => this.OnReceive(session,buffer);
            //session.DisconnectHandler =>() => this.RecyceSession(session);
            //session.CloseHandler = () => this.RecyceSession(session);

            // SocketAsync 与socket绑定
            session.Bind(socket);
            //添加到活动列表
            (this.AllSessions as ICollection<T>).Add(session);
            //通知已连接
            this.OnConnect(session);
            //开始接收数据
            session.TryReceive();
        }
    }
}
