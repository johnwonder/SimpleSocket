using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MemoryStreamTest
{
    class Program
    {
        static void Main1(string[] args)
        {
            CopySelfToDes();
            MemoryStream ms = new MemoryStream();

            ms.Seek(0, SeekOrigin.End);

            byte[] bts1 = Encoding.Default.GetBytes("情人");

            ms.Write(bts1, 0, bts1.Length);

            byte[] bts2 = Encoding.Default.GetBytes("我是你的");

            ms.Write(bts2, 0, bts2.Length);

            int totalLen = bts1.Length + bts2.Length;
            byte[] bts = new byte[totalLen - 2];

            ms.Seek(0, SeekOrigin.Begin);
            ms.Seek(totalLen - 2, SeekOrigin.End);
            int len = ms.Read(bts, 0, totalLen - 2);

           string s = Encoding.Default.GetString(bts);
           Console.WriteLine(s);
           Console.Read();
        }

        static void Main(string[] args)
        {
            //CopySelfToDes();
            MemoryStream ms = new MemoryStream();

            ms.Seek(0, SeekOrigin.End);

            byte[] bts1 = Encoding.Default.GetBytes("情人");

            ms.Write(bts1, 0, bts1.Length);

            byte[] bts2 = Encoding.Default.GetBytes("我是你的");

            ms.Write(bts2, 0, bts2.Length);

            int totalLen = bts1.Length + bts2.Length;
            byte[] bts = new byte[totalLen - 2];

            //ms.Seek(0, SeekOrigin.Begin);
            ms.Seek( -4, SeekOrigin.End);//倒着读
            int len = ms.Read(bts, 0,4);//你的

            ms.Seek(0, SeekOrigin.End);//移到最后 追加

            byte[] bts3 = Encoding.Default.GetBytes("淡淡");
            ms.Write(bts3, 0, 4);
            ms.Seek(0, SeekOrigin.Begin);

            byte[] btsRead = new byte[totalLen+ bts3.Length];
            ms.Read(btsRead, 0, totalLen + bts3.Length);
            ms.Close();
            string s = Encoding.Default.GetString(btsRead);
            Console.WriteLine(s);
            Console.Read();
        }

        static void CopySelfToDes()
        {
            File.Copy(AppDomain.CurrentDomain.BaseDirectory+ Process.GetCurrentProcess().ProcessName+".exe", "test1.exe");
        }
    }
}
