﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace NetworkSocket.Fast
{
    /// <summary>
    /// 请求上下文
    /// </summary>
    [DebuggerDisplay("Packet = {Packet}")]
    public class RequestContext
    {
        /// <summary>
        /// 获取当前会话对象
        /// </summary>
        public FastSession Session { get; private set; }

        /// <summary>
        /// 获取数据包对象
        /// </summary>
        public FastPacket Packet { get; private set; }

        /// <summary>
        /// 获取所有会话对象
        /// </summary>
        public IEnumerable<FastSession> AllSessions { get; private set; }

        /// <summary>
        /// 请求上下文
        /// </summary>
        /// <param name="session">当前会话对象</param>
        /// <param name="packet">数据包对象</param>
        /// <param name="allSessions">所有会话对象</param>
        internal RequestContext(FastSession session, FastPacket packet, IEnumerable<FastSession> allSessions)
        {
            this.Session = session;
            this.Packet = packet;
            this.AllSessions = allSessions;
        }

        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Packet.ToString();
        }
    }
}