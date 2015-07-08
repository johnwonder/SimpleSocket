using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace SimpleSocket
{
    public class ClientInfo
    {
        public byte[] buffer;
        public string NickName { get; set; }
        public EndPoint Id { get; set; }

        public IntPtr handle { get; set; }

        public string Name
        {

            get
            {

                if (!string.IsNullOrEmpty(NickName))
                    return NickName;
                else
                    return string.Format("{0}#{1}", Id, handle);
            }
        }
             
    }
}
