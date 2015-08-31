using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer
{
    internal class FreeSessionStack<T>:IDisposable where T : SimpleSessionBase
    {
        private ConcurrentStack<T> stack = new ConcurrentStack<T>();

        public int Count
        {
           get
            {
                return this.stack.Count;
            }
        }

        public void Add(T session)
        {
            this.stack.Push(session);
        }

        public T Take()
        {
            T session;
            if(this.stack.TryPop(out session))
            {
                return session;
            }
            return null;
        }

        public void Dispose()
        {
            var sessions = this.stack.ToArray();
            foreach (var session in sessions)
            {
                IDisposable disposable = session;
                disposable.Dispose();
            }
        }
    }
}
