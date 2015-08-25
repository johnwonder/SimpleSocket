using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSocket
{
    public class SocketMessage
    {
        public bool isLogin { get; set; }
        public ClientInfo Client { get; set; }
        public string Message { get; set; }

        public DateTime Time { get; set; }
    }
}
