using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleServer
{
    public class SimpleSessionBase:ISession,IDisposable
    {
        private Socket socket;

        private object socketRoot = new object();

        private bool socketClosed = true;

        private int pendingSendCount = 0;

        /// <summary>
        /// 用于发送的SocketAsyncEventArgs
        /// </summary>
        private SocketAsyncEventArgs sendArg = new SocketAsyncEventArgs();

        private object queueSync = new object();

        private ConcurrentQueue<ByteRange> byteRangeQueue = new ConcurrentQueue<ByteRange>();

        private ReceiveBuffer recvBuffer = new ReceiveBuffer();

        /// <summary>
        /// 处理和分析收到的数据的委托
        /// </summary>
        internal Action<ReceiveBuffer> ReceiveHandler;

        /// <summary>
        /// 链接断开委托
        /// </summary>
        internal Action DisconnectHandler;

        /// <summary>
        /// 关闭时的委托
        /// </summary>
        internal Action CloseHandler;


        public IPEndPoint RemoteEndPoint { get;private set; }

        public dynamic TagBag { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ITag TagData { get;private set; }

        public SessionExtraState ExtraState { get;private set; }

        public bool IsConnectd
        {
            get
            {
                return this.socket != null && this.socket.Connected;
            }
        }

        public SimpleSessionBase()
        {
           // this.sendArg.Completed += this.se

           // this.TagData = new 
        }
    }

    /// <summary>
    /// 表示会话接受到的历史数据
    /// 非线程安全类型
    /// 不可继承
    /// </summary>
    public sealed class ReceiveBuffer
	{
        /// <summary>
        /// 指针位置
        /// </summary>
        private int _position;

        /// <summary>
        /// 容量
        /// </summary>
        private int _capacity;

        /// <summary>
        /// 当前数据
        /// </summary>
        private byte[] _buffer;

        public readonly object SyncRoot = new object();

        /// <summary>
        /// 获取数据长度
        /// </summary>
        public int Length { get;private set; }

        public Endians Endian { get; set; }

        /// <summary>
        /// 获取或设置指针位置
        /// 为[0,Length]之间
        /// </summary>
        public int Position 
        {
            get
            {
                return this._position;
            }
            set
            {
                if(value < 0 || value > this.Length)
                {
                    throw new ArgumentOutOfRangeException("");
                }
                this._position =value;
            }
        }

         internal ReceiveBuffer ()
	     {
             this.Endian = Endians.Big;
             this._capacity = 1024;
             this._buffer = new byte[this._capacity];
	     }

        /// <summary>
        /// 添加指定数据数组
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        internal void Add(byte[] array,int offset, int count)
        {
            if(array == null || array.Length == 0)
            {
                return;
            }

            int newLength = this.Length + count;
            this.ExpandCapacity(newLength);
            Buffer.BlockCopy(array,offset,this._buffer,this.Length,count);
            this.Length = newLength;
        }

        private void ExpandCapacity(int newLength)
        {
            if(newLength <= this._capacity)
                return;
            while (newLength > this._capacity)
	        {
                this._capacity = this._capacity * 2;
	        }
            
            var newBuffer = new byte[this._capacity];
            if(this.Length > 0)
            {
                Buffer.BlockCopy(this._buffer,0,newBuffer,0,this.Length );

            }
            this._buffer = newBuffer;
        }

        public byte this[int index]
        {
            get
            {
                if(index < 0 || index >= this.Length)
                {
                    throw new ArgumentOutOfRangeException("");
                }
                return this._buffer[index];
            }
        }

        public bool ReadBoolean()
        {
            return  ReadByte() != 0;
            //return this
        }

        public byte ReadByte()
        {
            var value = this[this.Position];
            this.Position = this.Position + sizeof(byte);
            return value;
        }

        public short ReadInt16()
        {
            return 1;
           // var value = ByteConverter
        }
	}

    public static class ByteConverter
	{
        public static unsafe long ToInt64(byte[] bytes,int startIndex,Endians endian)
        {
            fixed(byte* pbyte = &bytes[startIndex])
            {
                if(endian == Endians.Little)
                {
                    int i1 = (*pbyte) | (*(pbyte+1) << 8) | (*(pbyte+2) << 16) | (*(pbyte+3) <<24);
                    int i2 = (*(pbyte+4))  | (*(pbyte+5) <<8) | (*(pbyte+6) << 16) | (*(pbyte+7) << 24);
                    return (uint)i1 | ((long)i2 << 32);
                }
                else
                {
                    int i1 = (*pbyte << 24) | (*(pbyte +1) << 16) | (*(pbyte+2) << 8) | (*(pbyte +3)) ;
                    int i2 = (*(pbyte+4) << 24) |(*(pbyte+5) << 16) | (*(pbyte+6) << 8) | (*(pbyte+7));
                    return (uint)i2 | ((long)i1 << 32);
                }
            }
        }

        public static ulong ToUInt64(byte[] bytes,int startIndex,Endians endian)
        {
            return (ulong)ToInt64(bytes,startIndex,endian);
        }

        public unsafe static int ToInt32(byte[] bytes,int startIndex,Endians endian)
        {
               fixed(byte* pbyte = &bytes[startIndex])
               {
                   if(endian== Endians.Little)
                   {
                       return (*pbyte) | (*(pbyte+1) << 8) | (*(pbyte+2) << 16) | (*(pbyte+3) << 24);
                   }
                   else
                   {
                       return (*pbyte << 24) | (*(pbyte + 1) << 16) | (*(pbyte + 2) << 8) | (*(pbyte + 3)); 
                   }
               }
        }

        /// <summary>
        /// 返回由字节数组中指定位置的四个字节转换来的16位无符号整数
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="startIndex">位置</param>    
        /// <param name="endian">高低位</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static ushort ToUInt16(byte[] bytes, int startIndex, Endians endian)
        {
            return (ushort)ToInt16(bytes, startIndex, endian);
        }


        /// <summary>
        /// 返回由64位有符号整数转换为的字节数组
        /// </summary>
        /// <param name="value">整数</param>    
        /// <param name="endian">高低位</param>
        /// <returns></returns>
        public unsafe static byte[] ToBytes(long value, Endians endian)
        {
            byte[] bytes = new byte[8];
            fixed (byte* pbyte = &bytes[0])
            {
                if (endian == Endians.Little)
                {
                    *pbyte = (byte)(value);
                    *(pbyte + 1) = (byte)(value >> 8);
                    *(pbyte + 2) = (byte)(value >> 16);
                    *(pbyte + 3) = (byte)(value >> 24);
                    *(pbyte + 4) = (byte)(value >> 32);
                    *(pbyte + 5) = (byte)(value >> 40);
                    *(pbyte + 6) = (byte)(value >> 48);
                    *(pbyte + 7) = (byte)(value >> 56);
                }
                else
                {
                    *(pbyte + 7) = (byte)(value);
                    *(pbyte + 6) = (byte)(value >> 8);
                    *(pbyte + 5) = (byte)(value >> 16);
                    *(pbyte + 4) = (byte)(value >> 24);
                    *(pbyte + 3) = (byte)(value >> 32);
                    *(pbyte + 2) = (byte)(value >> 40);
                    *(pbyte + 1) = (byte)(value >> 48);
                    *pbyte = (byte)(value >> 56);
                }
            }
            return bytes;
        }


        /// <summary>
        /// 返回由64位无符号整数转换为的字节数组
        /// </summary>
        /// <param name="value">整数</param>    
        /// <param name="endian">高低位</param>
        /// <returns></returns>
        public static byte[] ToBytes(ulong value, Endians endian)
        {
            return ToBytes((long)value, endian);
        }

        /// <summary>
        /// 返回由32位有符号整数转换为的字节数组
        /// </summary>
        /// <param name="value">整数</param>    
        /// <param name="endian">高低位</param>
        /// <returns></returns>
        public unsafe static byte[] ToBytes(int value, Endians endian)
        {
            byte[] bytes = new byte[4];
            fixed (byte* pbyte = &bytes[0])
            {
                if (endian == Endians.Little)
                {
                    *pbyte = (byte)(value);
                    *(pbyte + 1) = (byte)(value >> 8);
                    *(pbyte + 2) = (byte)(value >> 16);
                    *(pbyte + 3) = (byte)(value >> 24);
                }
                else
                {
                    *(pbyte + 3) = (byte)(value);
                    *(pbyte + 2) = (byte)(value >> 8);
                    *(pbyte + 1) = (byte)(value >> 16);
                    *pbyte = (byte)(value >> 24);
                }
            }
            return bytes;
        }

        /// <summary>
        /// 返回由32位无符号整数转换为的字节数组
        /// </summary>
        /// <param name="value">整数</param>    
        /// <param name="endian">高低位</param>
        /// <returns></returns>
        public static byte[] ToBytes(uint value, Endians endian)
        {
            return ToBytes((int)value, endian);
        }

        /// <summary>
        /// 返回由16位有符号整数转换为的字节数组
        /// </summary>
        /// <param name="value">整数</param>    
        /// <param name="endian">高低位</param>
        /// <returns></returns>
        public unsafe static byte[] ToBytes(short value, Endians endian)
        {
            byte[] bytes = new byte[2];
            fixed (byte* pbyte = &bytes[0])
            {
                if (endian == Endians.Little)
                {
                    *pbyte = (byte)(value);
                    *(pbyte + 1) = (byte)(value >> 8);
                }
                else
                {
                    *(pbyte + 1) = (byte)(value);
                    *pbyte = (byte)(value >> 8);
                }
            }
            return bytes;
        }

        /// <summary>
        /// 返回由16位无符号整数转换为的字节数组
        /// </summary>
        /// <param name="value">整数</param>    
        /// <param name="endian">高低位</param>
        /// <returns></returns>
        public static byte[] ToBytes(ushort value, Endians endian)
        {
            return ToBytes((short)value, endian);             
        }
	}

    public enum Endians
	{
        /// <summary>
        /// 高位在前
        /// </summary>
        Big,
        /// <summary>
        /// 低位在前
        /// </summary>
        Little
	}

    interface ISession
    {
        ITag TagData { get; }

        dynamic TagBag { get; }

        IPEndPoint RemoteEndPoint { get; }

        bool IsConnected { get; }

        void Send(ByteRange byteRange);

        void Close();
    }

    /// <summary>
    /// 用户附加数据接口
    /// </summary>
    public interface ITag
    {
        void Set(string key, object value);

        bool IsExist(string key);

        object TryGet(string key);

        T TryGet<T>(string key);

        T TryGet<T>(string key, T defaultValue);

        bool TryGet(string key, out object value);

        bool Remove(string key);

        void Clear();
    }


    public sealed class ByteRange
    {
        /// <summary>
        /// 获取偏移量
        /// </summary>
        public int Offset { get;private set; }

        /// <summary>
        /// 获取字节数
        /// </summary>
        public int Count { get;private set; }

        /// <summary>
        /// 获取字节数组
        /// </summary>
        public byte[] Buffer { get;private set; }

        /// <summary>
        /// 表示字节数组范围
        /// </summary>
        /// <param name="buffer"></param>
        public ByteRange(byte[] buffer)
        {
            if(buffer == null)
            {
                throw new ArgumentNullException();
            }
            this.Buffer = buffer;
            this.Count = buffer.Length;
            this.Offset = 0;
        }

        public ByteRange(byte[] buffer,int offset,int count)
        {
            if(buffer == null)
            {
                throw new ArgumentNullException();
            }

            if(offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("");
            }

            if(count < 0 || (offset +count ) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("");
            }
            this.Buffer = buffer;
            this.Offset = offset;
            this.Count = count;
        }

        public IEnumerable<ByteRange> SplitBySize(int size)
        {
            if(size >= this.Count)
            {
                yield return this;
                yield break;
            }

            var remain = this.Count % size;
            var count = this.Count - remain;

            var offset = 0;
            while (offset < count)
            {
                yield return new ByteRange(this.Buffer, this.Offset + offset, size);
                offset = offset + size;
            }

            if(remain > 0)
            {
                yield return new ByteRange(this.Buffer, offset, remain);
            }
        }

        private class DebugView
        {
            private ByteRange view;

            public DebugView(ByteRange view)
            {
                this.view = view;
            }

            public byte[] Values 
            {
                get
                {
                    var byteArray = new byte[this.view.Count];
                    System.Buffer.BlockCopy(this.view.Buffer,this.view.Offset,byteArray,0,this.view.Count);
                    return byteArray;
                }
                
            }
        }
    }

    sealed class SessionExtraState
    {
        /// <summary>
        /// 发送字节数
        /// </summary>
        private long sendByteCount = 0;

        /// <summary>
        /// 接受字节数
        /// </summary>
        private long recvByteCount = 0;

        /// <summary>
        /// 发送总次数
        /// </summary>
        private long sendTimes = 0;

        private long recvTimes = 0;

        /// <summary>
        /// 获取连接成功的时间
        /// </summary>
        public DateTime ConnectedTime { get;private set; }

        /// <summary>
        /// 获取最近一次发送数据的时间
        /// </summary>
        public DateTime LastSendTime { get;private set; }

        /// <summary>
        /// 获取最近一次发送数据的时间
        /// </summary>
        public DateTime LastRecvTime { get; private set; }
        /// <summary>
        /// 获取最大发送的数据包字节数
        /// </summary>
        public int MaxSendSize { get;private set; }

        /// <summary>
        /// 获取最大接受的数据包字节数
        /// </summary>
        public int MaxRecvSize { get;private set; }
        /// <summary>
        /// 获取最小发送的数据包字节数
        /// </summary>
        public int MinSendSize { get; private set; }

        /// <summary>
        /// 获取最小接受的数据包字节数
        /// </summary>
        public int MinRecvSize { get; private set; }

        public long SendTimes
        {
            get
            {
                return this.sendTimes;
            }
        }

        public long RecvTimes
        {
            get
            {
                return this.recvTimes;
            }
        }
        public long TotalSendByteCount
        {
            get
            {
                return this.sendByteCount;
            }
        }

        public long TotalRecvByteCount
        {
            get
            {
                return this.recvByteCount;
            }
        }

        internal SessionExtraState()
        {

        }

        internal void SetBinded()
        {
            this.sendByteCount = 0;
            this.recvByteCount = 0;
            this.sendTimes = 0;
            this.recvTimes = 0;
            this.ConnectedTime = DateTime.Now;
        }

        internal void SetSended(int byteCount)
        {
            Interlocked.Increment(ref this.sendTimes);
            Interlocked.Add(ref this.sendByteCount, byteCount);

            if (this.MaxSendSize < byteCount)
                this.MaxSendSize = byteCount;

            if(this.MinSendSize == 0 || this.MinSendSize > byteCount)
            {
                this.MinSendSize = byteCount;
            }
        }

        internal void SetRecved(int byteCount)
        {
            Interlocked.Increment(ref this.recvByteCount);
            Interlocked.Add(ref this.recvByteCount, byteCount);

            this.LastRecvTime = DateTime.Now;

            if (this.MaxRecvSize < byteCount)
            {
                this.MaxRecvSize = byteCount;
            }

            if (this.MinRecvSize == 0 || this.MinRecvSize > byteCount)
            {
                this.MinRecvSize = byteCount;
            }
        }
    }

    internal class TagData:ITag
    {
        private ConcurrentDictionary<string, object> dic = new ConcurrentDictionary<string, object>();

        public IEnumerable<KeyValuePair<string,object>> KeyValues
        {
            get
            {
                return this.dic;
            }
        }

        public void Set(string key,object value)
        {
            this.dic.AddOrUpdate(key, value, (k, v) => value);
        }

        public bool IsExist(string key)
        {
            return this.dic.ContainsKey(key);
        }

        public object TryGet(string key)
        {
            object value;
            this.TryGet(key, out value);
            return value;
        }

        public T TryGet<T>(string key)
        {
            object value;
            if (this.TryGet(key, out value) == false)
            {
                return default(T);
            }
            try
            {
                return (T)value;
            }
            catch (Exception)
            {

                return default(T);
            }
            
        }

        public T TryGet<T>(string key,T defaultValue)
        {
            object value;
            if(this.TryGet(key,out value))
            {
                return (T)value;
            }
            return defaultValue;
        }

        public bool TryGet(string key,out object value)
        {
            return this.dic.TryGetValue(key, out value);
        }

        public bool Remove(string key)
        {
            object value;
            return this.dic.TryRemove(key, out value);
        }

        public void Clear()
        {
            this.dic.Clear();
        }

        private class DebugView
        {
            private TagData view;

            public DebugView(TagData view)
            {
                this.view = view;
            }


            public KeyValuePair<string,object>[] values
            {
                 get
                {
                    return this.view.KeyValues.ToArray();
                }
            }
        }
    }
}
