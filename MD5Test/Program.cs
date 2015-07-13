using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace MD5Test
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] result = Encoding.Default.GetBytes("a");    //tbPass为输入密码的文本框
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);

            string s = bintoascii(output);
        }


        public static String bintoascii(byte[]  bySourceByte)
		{
			int len,i;
			byte tb;
			char high,tmp,low;
			string result= string.Empty;
			len=bySourceByte.Length;
			for(i=0;i<len;i++)
			{
				tb=bySourceByte[i];
				
				tmp=(char)(rightMove(tb,4)&0x000f);
				if(tmp>=10)
					high=(char)('a'+tmp-10);
				else
					high=(char)('0'+tmp);
				result+=high;
				tmp=(char)(tb&0x000f);
				if(tmp>=10)
					low=(char)('a'+tmp-10);
				else
					low=(char)('0'+tmp);
				
				result+=low;
			}
			return result;
		}

      static  int rightMove(byte value, int pos)
      {

          if (pos != 0)  //移动 0 位时直接返回原值
          {
              byte mask = (byte)0x7f;     // int.MaxValue = 0x7FFFFFFF 整数最大值
              value >>= 1;     //第一次做右移，把符号也算上，无符号整数最高位不表示正负
                                        //但操作数还是有符号的，有符号数右移1位，正数时高位补0，负数时高位补1
              value &= mask;     //和整数最大值进行逻辑与运算，运算后的结果为忽略表示正负值的最高位
              value >>= pos-1;     //逻辑运算后的值无符号，对无符号的值直接做右移运算，计算剩下的位
         }
          return value;
      }

      public static int MoveByte(int value, int pos)
      {
          if (value < 0)
          {
              string s = Convert.ToString(value, 2);    // 转换为二进制
              for (int i = 0; i < pos; i++)
              {
                  s = "0" + s.Substring(0, 31);
              }
              return Convert.ToInt32(s, 2);            // 将二进制数字转换为数字
          }
          else
          {
              return value >> pos;
          }
      }
    }
}
