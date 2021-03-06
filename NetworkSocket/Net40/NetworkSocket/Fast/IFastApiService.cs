﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkSocket.Fast
{
    /// <summary>
    /// 定义快速服务的执行
    /// </summary>
    public interface IFastApiService : IDisposable
    {
        /// <summary>
        /// 执行Api行为
        /// </summary>              
        /// <param name="actionContext">Api行为上下文</param>      
        void Execute(ActionContext actionContext);
    }
}
