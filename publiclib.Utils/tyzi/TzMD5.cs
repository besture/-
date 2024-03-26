using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace publiclib.Utils
{
    public class TzMD5
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <param name="encode">字符的编码</param>
        /// <returns></returns>
        public static string MD5Encrypt(string input, Encoding encode)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(encode.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// SHA256函数
        /// </summary>
        /// /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string SHA256(string str, Encoding encode)
        {
            byte[] SHA256Data = encode.GetBytes(str);
            SHA256Managed Sha256 = new SHA256Managed();
            byte[] Result = Sha256.ComputeHash(SHA256Data);
            return Convert.ToBase64String(Result);  //返回长度为44字节的字符串
        }

        /// <summary>
        /// HMACSHA1算法加密并返回ToBase64String
        /// </summary>
        /// <param name="EncryptText">签名参数字符串</param>
        /// <param name="EncryptKey">密钥参数</param>
        /// <returns>返回一个签名值(即哈希值)</returns>
        public static string HMACSHA1Text(string EncryptText, string EncryptKey, Encoding encode)
        {
            //HMACSHA1加密
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = encode.GetBytes(EncryptKey);
            byte[] dataBuffer = encode.GetBytes(EncryptText);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// SHA1加密
        /// </summary>
        public static string EncryptSHA1(string str)
        {
            using (HashAlgorithm hashAlgorithm = new SHA1CryptoServiceProvider())
            {
                byte[] utf8Bytes = Encoding.UTF8.GetBytes(str);
                byte[] hashBytes = hashAlgorithm.ComputeHash(utf8Bytes);
                StringBuilder builder = new StringBuilder();
                foreach (byte hashByte in hashBytes)
                {
                    builder.AppendFormat("{0:x2}", hashByte);
                }
                return builder.ToString();
            }
        }
    }
}
