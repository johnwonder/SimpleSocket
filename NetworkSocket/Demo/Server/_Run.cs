﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.Collections.Concurrent;
using NetworkSocket.Policies;
using NetworkSocket;
using System.IO;
using System.Reflection;
using NetworkSocket.Fast;
using Server.Services;
using Server.Filters;

namespace Server
{
    class _Run
    {
        static void Main(string[] args)
        {
            var fastServer = new FastServer();
            
            fastServer.GlobalFilter.Add(new ExceptionFilterAttribute());           
            fastServer.BindService(fastServer.GetType().Assembly);
            fastServer.RegisterResolver();
            fastServer.StartListen(1380);

            //设置命令行标题
            Console.Title = "FastServer V" + new SystemService().GetVersion();
            Console.WriteLine("服务已启动，端口：" + fastServer.LocalEndPoint.Port);
            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
