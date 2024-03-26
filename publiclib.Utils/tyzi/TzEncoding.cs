using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace publiclib.Utils
{
    /// <summary>
    /// 编码格式错误
    /// </summary>
    public class TzEncoding
    {
        /// <summary>
        /// GB2312转换成UTF8
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string gb2312_utf8(string text)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            //声明字符集   
            System.Text.Encoding utf8 = System.Text.Encoding.GetEncoding("utf-8");
            System.Text.Encoding gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            var gb = gb2312.GetBytes(text);
            var gb2 = System.Text.Encoding.Convert(gb2312, utf8, gb);
            //返回转换后的字符   
            return utf8.GetString(gb2);
        }

        /// <summary>
        /// UTF8转换成GB2312
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string utf8_gb2312(string text)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            //声明字符集   
            System.Text.Encoding utf8 = System.Text.Encoding.GetEncoding("utf-8");
            System.Text.Encoding gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            var utf = utf8.GetBytes(text);
            var utf2 = System.Text.Encoding.Convert(utf8, gb2312, utf);
            //返回转换后的字符   
            return gb2312.GetString(utf2);
        }

        /// <summary>
        /// UTF8转换成ASCII
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string utf8_ascii(string text)
        {
            System.Text.Encoding utf8, ascii;
            utf8 = System.Text.Encoding.GetEncoding("utf-8");
            ascii = System.Text.Encoding.GetEncoding("ascii");
            byte[] utf;
            utf = utf8.GetBytes(text);
            utf = System.Text.Encoding.Convert(utf8, ascii, utf);
            return ascii.GetString(utf);
        }

        /// <summary>
        /// ASCII转换成UTF8
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ascii_utf8(string text)
        {
            System.Text.Encoding utf8, ascii;
            utf8 = System.Text.Encoding.GetEncoding("utf-8");
            ascii = System.Text.Encoding.GetEncoding("ascii");

            byte[] bt;
            bt = ascii.GetBytes(text);
            bt = System.Text.Encoding.Convert(ascii, utf8, bt);
            return utf8.GetString(bt);
        }

        public static string gb2312Code(string text)
        {
            StringBuilder sb = new StringBuilder();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            //声明字符集   
            System.Text.Encoding utf8 = System.Text.Encoding.GetEncoding("utf-8");
            System.Text.Encoding gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            var utf = utf8.GetBytes(text);
            var utf2 = System.Text.Encoding.Convert(utf8, gb2312, utf);
            foreach (Byte b in utf2)
            {
                string s = Convert.ToString(b, 16);
                if (s == "0") continue;
                sb.Append(string.Format("0x{0}", s));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取字符串16进制编码
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isSeparator"></param>
        /// <returns></returns>
        public static string getUnicode16(string text, bool isSeparator = true)
        {
            string result = string.Empty;
            var chars = text.ToArray();
            foreach (var item in chars)
            {
                var bts = Encoding.Unicode.GetBytes(item.ToString());

                string lowcode = Convert.ToString(bts[0], 16);
                if (lowcode.Length == 1) lowcode = $"0{lowcode}";

                string hightcode = Convert.ToString(bts[1], 16);
                if (hightcode.Length == 1) hightcode = $"0{hightcode}";

                if (!isSeparator)
                    result += $"{hightcode}{lowcode}";
                else
                    result += $"&#x{hightcode}{lowcode};";
            }
            return result;
        }
    }
}
